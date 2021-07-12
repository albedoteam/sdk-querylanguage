namespace AlbedoTeam.Sdk.QueryLanguage.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Exceptions;

    internal static class ExpressionConversions
    {
        private static readonly Dictionary<Type, Type[]> ImplicitConversions = new Dictionary<Type, Type[]>
        {
            {
                typeof(sbyte),
                new[]
                {
                    typeof(sbyte),
                    typeof(short),
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(byte),
                new[]
                {
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(short),
                new[]
                {
                    typeof(short),
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(ushort),
                new[]
                {
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(int),
                new[]
                {
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(uint),
                new[]
                {
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(long),
                new[]
                {
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(char),
                new[]
                {
                    typeof(char),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(float),
                new[]
                {
                    typeof(float),
                    typeof(double),
                    // not safe, but convenient
                    typeof(decimal)
                }
            },
            {
                typeof(ulong),
                new[]
                {
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(double),
                new[]
                {
                    typeof(double),
                    // not safe, but convenient
                    typeof(decimal)
                }
            },
            {
                typeof(decimal),
                new[]
                {
                    typeof(decimal)
                }
            }
        };

        private static readonly Type[] EnumConversions =
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong)
        };

        private static bool TryGetCommonType(Type type1, Type type2, out Type commonType)
        {
            commonType = null;

            if (!ImplicitConversions.TryGetValue(type1, out var possibleType1))
                return false;

            if (!ImplicitConversions.TryGetValue(type2, out var possibleType2))
                return false;

            commonType = possibleType1.Intersect(possibleType2).FirstOrDefault();
            return commonType != null;
        }

        public static bool TryBoolean(ref Expression exp)
        {
            if (exp.Type == typeof(bool))
                return true;

            var left = exp;
            var right = (Expression) Expression.Constant(true);
            if (!TryImplicitlyConvert(ref left, ref right))
                return false;

            exp = Expression.Equal(left, right);
            return true;
        }

        public static bool TryEnumNumberConvert(ref Expression exp1, ref Expression exp2)
        {
            // same type, nothing to do here
            if (exp1.Type == exp2.Type)
                return true;

            var type1 = Nullable.GetUnderlyingType(exp1.Type) ?? exp1.Type;
            var type2 = Nullable.GetUnderlyingType(exp2.Type) ?? exp2.Type;

            if (EnumConversions.Contains(type2) && type1.GetTypeInfo().IsEnum)
            {
                exp2 = Convert(exp2, exp1.Type);
                return true;
            }

            if (!EnumConversions.Contains(type1) || !type2.GetTypeInfo().IsEnum)
                return false;

            exp1 = Convert(exp1, exp2.Type);
            return true;
        }

        public static bool TryEnumStringConvert(ref Expression exp1, ref Expression exp2, bool ignoreCase)
        {
            //same type, nothing to do here
            if (exp1.Type == exp2.Type)
                return true;

            var type1 = Nullable.GetUnderlyingType(exp1.Type) ?? exp1.Type;
            var type2 = Nullable.GetUnderlyingType(exp2.Type) ?? exp2.Type;

            if (type2 == typeof(string) && type1.GetTypeInfo().IsEnum)
                return ConvertStringExpression(exp1.Type, type1, ref exp2, ignoreCase);

            if (type1 == typeof(string) && type2.GetTypeInfo().IsEnum)
                return ConvertStringExpression(exp2.Type, type2, ref exp1, ignoreCase);

            return false;
        }

        private static bool ConvertStringExpression(
            Type enumType,
            Type enumUnderlyingType,
            ref Expression stringExpression,
            bool ignoreCase)
        {
            var isNullable = enumType != enumUnderlyingType;

            //we will only support string constants to convert to enums. This prevents having enum
            //parse errors while evaluating the final expression
            if (!ExpressionEvaluator.TryEvaluate(stringExpression, out string stringConstant))
                throw new NotSupportedException(
                    "Only string constants can be converted to an enum. String expressions can not be parsed as an enum.");

            //Enum.Parse will fail for null, however if we have a nullable enum a null is valid
            if (stringConstant == null && isNullable)
            {
                stringExpression = Convert(Expression.Constant(null), enumType);
                return true;
            }

            try
            {
                var parsedEnum = Enum.Parse(enumUnderlyingType, stringConstant!, ignoreCase);
                stringExpression = Convert(Expression.Constant(parsedEnum), enumType);
                return true;
            }
            catch (ArgumentException ex)
            {
                //enum parse failures give unhelpful errors, we will catch and rethrow an error with
                //some more details
                throw new EnumParseException(stringConstant, enumType, ex);
            }
        }

        public static bool TryImplicitlyConvert(ref Expression exp1, ref Expression exp2)
        {
            var type1 = exp1.Type;
            var type2 = exp2.Type;

            //same type, nothing to do here
            if (type1 == type2)
                return true;

            var nullableType1 = Nullable.GetUnderlyingType(exp1.Type);
            var nullableType2 = Nullable.GetUnderlyingType(exp2.Type);
            var isType1Nullable = nullableType1 != null || !type1.GetTypeInfo().IsValueType;
            var isType2Nullable = nullableType2 != null || !type2.GetTypeInfo().IsValueType;

            type1 = nullableType1 ?? type1;
            type2 = nullableType2 ?? type2;

            Type commonType;
            if ((isType1Nullable || isType2Nullable) && type1 == type2)
            {
                //if the underlying type is the same, the common type is
                //just the nullable version
                commonType = typeof(Nullable<>).MakeGenericType(type1);
            }
            else if (isType1Nullable && IsNullConstant(exp2))
            {
                //null constants always have type object,
                //if other type allows null convert it to the other type
                commonType = exp1.Type;
            }
            else if (isType2Nullable && IsNullConstant(exp1))
            {
                //null constants always have type object,
                //if other type allows null convert it to the other type
                commonType = exp2.Type;
            }
            else if (TryGetCommonType(type1, type2, out commonType))
            {
                //we have a common type, if we had a nullable type to begin
                //with convert our common type to a nullable as well
                if (isType1Nullable || isType2Nullable)
                    commonType = typeof(Nullable<>).MakeGenericType(commonType);
            }
            else
            {
                return false;
            }

            exp1 = Convert(exp1, commonType);
            exp2 = Convert(exp2, commonType);

            return true;
        }

        public static Expression Convert(Expression exp, Type type)
        {
            return exp.Type == type
                ? exp
                : Expression.Convert(exp, type);
        }

        public static bool IsNullConstant(Expression exp)
        {
            if (exp is ConstantExpression constantExpression)
                return constantExpression.Value == null;

            return false;
        }
    }
}
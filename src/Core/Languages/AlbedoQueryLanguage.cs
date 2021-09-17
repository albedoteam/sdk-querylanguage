namespace AlbedoTeam.Sdk.QueryLanguage.Core.Languages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Definitions;
    using Definitions.Brackets;
    using Enumerators;
    using Injections;
    using Structs;
    using Utility;

    public sealed class AlbedoQueryLanguage
    {
        private readonly BaseQueryLanguage _language;
        public readonly IAqlResolver Resolver;

        public AlbedoQueryLanguage(IAqlResolver resolver)
        {
            Resolver = resolver;
            _language = new BaseQueryLanguage(AllDefinitions().ToArray());
        }

        public AqlFormula Parse(string text)
        {
            var (body, operands) = _language.Parse(text);
            body = ExpressionConversions.Convert(body, typeof(object));

            var expressionFunction = Expression.Lambda<Func<object>>(body);
            var function = expressionFunction.Compile();
            var result = function();

            var frml = new AqlFormula
            {
                ExpressionText = text,
                Result = result
            };

            foreach (var (_, value) in operands)
                if (value.InnerDep != null)
                    frml.InnerOps.Add(value);

            return frml;
        }

        private IEnumerable<GrammarDefinition> AllDefinitions()
        {
            IEnumerable<FunctionCallDefinition> functions;
            var definitions = new List<GrammarDefinition>();

            definitions.AddRange(TypeDefinitions());
            definitions.AddRange(functions = FunctionDefinitions());
            definitions.AddRange(BracketDefinitions(functions));
            definitions.AddRange(LogicalOperatorDefinitions());
            definitions.AddRange(ArithmeticOperatorDefinitions());
            definitions.AddRange(PropertyDefinitions());
            definitions.AddRange(WhitespaceDefinitions());
            return definitions;
        }

        private IEnumerable<GrammarDefinition> TypeDefinitions()
        {
            return new[]
            {
                new OperandDefinition(
                    new Grammar("GUID",
                        @"[0-9A-Fa-f]{8}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{12}"),
                    x => Expression.Constant(Guid.Parse(x.Trim('\'')))),

                new OperandDefinition(
                    new Grammar("OBJECTID",
                        @"[a-f\d]{24}"),
                    x => Expression.Constant(x.Trim('\''))),

                new OperandDefinition(
                    new Grammar("STRING",
                        @"'(?:\\.|[^'])*'"),
                    x => Expression.Constant(x.Trim('\'')
                        .Replace("\\'", "'")
                        .Replace("\\r", "\r")
                        .Replace("\\f", "\f")
                        .Replace("\\n", "\n")
                        .Replace("\\\\", "\\")
                        .Replace("\\b", "\b")
                        .Replace("\\t", "\t"))),

                new OperandDefinition(
                    new Grammar("DECIMAL", @"\-?\d+(\.\d+)?"),
                    x => Expression.Constant(decimal.Parse(x))),

                new OperandDefinition(
                    new Grammar("PI", @"[Pp][Ii]"),
                    x => Expression.Constant(Math.PI))
            };
        }

        private IEnumerable<GrammarDefinition> LogicalOperatorDefinitions()
        {
            return new GrammarDefinition[]
            {
                new BinaryOperatorDefinition(
                    new Grammar("EQ", @"\b(eq)\b"),
                    11,
                    ConvertEnumsIfRequired(Expression.Equal)),

                new BinaryOperatorDefinition(
                    new Grammar("NE", @"\b(ne)\b"),
                    12,
                    ConvertEnumsIfRequired(Expression.NotEqual)),

                new BinaryOperatorDefinition(
                    new Grammar("GT", @"\b(gt)\b"),
                    13,
                    Expression.GreaterThan),

                new BinaryOperatorDefinition(
                    new Grammar("GE", @"\b(ge)\b"),
                    14,
                    Expression.GreaterThanOrEqual),

                new BinaryOperatorDefinition(
                    new Grammar("LT", @"\b(lt)\b"),
                    15,
                    Expression.LessThan),

                new BinaryOperatorDefinition(
                    new Grammar("LE", @"\b(le)\b"),
                    16,
                    Expression.LessThanOrEqual),

                new BinaryOperatorDefinition(
                    new Grammar("AND", @"\b(and)\b"),
                    19,
                    Expression.And),

                new BinaryOperatorDefinition(
                    new Grammar("OR", @"\b(or)\b"),
                    20,
                    Expression.Or),

                new UnaryOperatorDefinition(
                    new Grammar("NOT", @"\b(not)\b"),
                    21,
                    RelativePosition.Right,
                    arg =>
                    {
                        ExpressionConversions.TryBoolean(ref arg);
                        return Expression.Not(arg);
                    })
            };
        }

        private IEnumerable<FunctionCallDefinition> FunctionDefinitions()
        {
            return new[]
            {
                new FunctionCallDefinition(
                    new Grammar("FN_CONVERSION", @"[Ff][Xx][(]"),
                    new[] { typeof(string), typeof(decimal), typeof(IAqlResolver) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => AqlFunctions.ApplyConversion(null, 0, null)),
                            new[] { parameters[0], parameters[1], Expression.Constant(this) });
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_IF", @"[Ii][Ff][(]"),
                    new[] { typeof(bool), typeof(decimal), typeof(decimal), typeof(IAqlResolver) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => AqlFunctions.ApplyIf(false, 0, 0, null)), parameters[0],
                            parameters[1], parameters[2], Expression.Constant(this));
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_EACH", @"[Ee][Aa][Cc][Hh][(]"),
                    new[] { typeof(List<decimal>), typeof(string), typeof(IAqlResolver) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => AqlFunctions.ApplyEach(null, null, null)),
                            new[] { parameters[0], parameters[1], Expression.Constant(this) });
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_SIN", @"[Ss][Ii][Nn]\("),
                    new[] { typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Sin(0)), parameters[0]);
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_COS", @"[Cc][Oo][Ss]\("),
                    new[] { typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Cos(0)), parameters[0]);
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_TAN", @"[Tt][Aa][Nn]\("),
                    new[] { typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Tan(0)), parameters[0]);
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_SQRT", @"[Ss][Qq][Rr][Tt]\("),
                    new[] { typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Sqrt(0)), parameters[0]);
                    }),

                new FunctionCallDefinition(new Grammar("FN_POW", @"[Pp][Oo][Ww]\("),
                    new[] { typeof(double), typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Pow(0, 0)), new[] { parameters[0], parameters[1] });
                    })
            };
        }

        private IEnumerable<GrammarDefinition> BracketDefinitions(
            IEnumerable<FunctionCallDefinition> functionCalls)
        {
            ParenthesisBracketOpenDefinition openParenthesisBracket;
            ListDelimiterDefinition delimeter;

            SquareBracketOpenDefinition openSquareBracket;
            AngleBracketOpenDefinition openAngleBracket;

            return new GrammarDefinition[]
            {
                openParenthesisBracket = new ParenthesisBracketOpenDefinition(
                    new Grammar("OPEN_PARENTHESIS_BRACKET", @"\(")),

                delimeter = new ListDelimiterDefinition(
                    new Grammar("COMMA", ",")),

                new ParenthesisBracketCloseDefinition(
                    new Grammar("CLOSE_PARENTHESIS_BRACKET", @"\)"),
                    new[] { openParenthesisBracket }.Concat(functionCalls),
                    delimeter),

                openSquareBracket = new SquareBracketOpenDefinition(
                    new Grammar("OPEN_SQUARE_BRACKET", @"\["),
                    this),

                new SquareBracketCloseDefinition(
                    new Grammar("CLOSE_SQUARE_BRACKET", @"\]"),
                    new[] { openSquareBracket }, delimeter),

                openAngleBracket = new AngleBracketOpenDefinition(
                    new Grammar("OPEN_ANGLE_BRACKET", @"\<"),
                    this),

                new AngleBracketCloseDefinition(
                    new Grammar("CLOSE_ANGLE_BRACKET", @"\>"),
                    new[] { openAngleBracket })
            };
        }

        private IEnumerable<GrammarDefinition> ArithmeticOperatorDefinitions()
        {
            return new[]
            {
                new BinaryOperatorDefinition(
                    new Grammar("ADD", @"\+"),
                    2,
                    Expression.Add),

                new BinaryOperatorDefinition(
                    new Grammar("SUB", @"\-"),
                    2,
                    Expression.Subtract),

                new BinaryOperatorDefinition(
                    new Grammar("MUL", @"\*"),
                    1,
                    Expression.Multiply),

                new BinaryOperatorDefinition(
                    new Grammar("DIV", @"\/"),
                    1,
                    Expression.Divide),

                new BinaryOperatorDefinition(
                    new Grammar("MOD", @"%"),
                    1,
                    Expression.Modulo)
            };
        }

        private IEnumerable<GrammarDefinition> PropertyDefinitions()
        {
            return new[]
            {
                new OperandDefinition(
                    new Grammar("PROPERTY_PATH", @"(?<![0-9])([A-Za-z_][A-Za-z0-9_]*\.?)+"),
                    (value, parameters) =>
                    {
                        return value.Split('.').Aggregate(
                            (Expression)parameters[0],
                            (exp, prop) => Expression.MakeMemberAccess(exp, TypeShim.GetProperty(exp.Type, prop)));
                    })
            };
        }

        private IEnumerable<GrammarDefinition> WhitespaceDefinitions()
        {
            return new[]
            {
                new WhitespaceDefinition(new Grammar("WHITESPACE", @"\s+", true))
            };
        }

        /// <summary>
        ///     Wraps the function to convert any constants to enums if required
        /// </summary>
        /// <param name="expFn">Function to wrap</param>
        /// <returns></returns>
        private Func<Expression, Expression, Expression> ConvertEnumsIfRequired(
            Func<Expression, Expression, Expression> expFn)
        {
            return (left, right) =>
            {
                var unused = ExpressionConversions.TryEnumNumberConvert(ref left, ref right)
                             || ExpressionConversions.TryEnumStringConvert(ref left, ref right, true);

                return expFn(left, right);
            };
        }
    }
}
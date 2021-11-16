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

    public sealed class AlbedoQueryLanguage<TContext>
        where TContext : IResolverContext
    {
        private readonly BaseQueryLanguage<TContext> _language;
        public readonly IAqlResolver<TContext> Resolver;

        public AlbedoQueryLanguage(IAqlResolver<TContext> resolver)
        {
            Resolver = resolver;
            _language = new BaseQueryLanguage<TContext>(AllDefinitions().ToArray());
        }

        public TContext Context { get; private set; }

        public ParseResponse<TContext> Parse(ParseRequest<TContext> request)
        {
            Context = request.Context;

            var body = _language.Parse(request);
            body = ExpressionConversions.Convert(body, typeof(object));

            var expressionFunction = Expression.Lambda<Func<object>>(body);
            var function = expressionFunction.Compile();
            var result = function();

            var response = new ParseResponse<TContext>
            {
                Request = request,
                Result = result
            };

            return response;
        }

        private IEnumerable<GrammarDefinition<TContext>> AllDefinitions()
        {
            IEnumerable<FunctionCallDefinition<TContext>> functions;
            var definitions = new List<GrammarDefinition<TContext>>();

            definitions.AddRange(TypeDefinitions());
            definitions.AddRange(functions = FunctionDefinitions());
            definitions.AddRange(BracketDefinitions(functions));
            definitions.AddRange(LogicalOperatorDefinitions());
            definitions.AddRange(ArithmeticOperatorDefinitions());
            definitions.AddRange(PropertyDefinitions());
            definitions.AddRange(WhitespaceDefinitions());
            return definitions;
        }

        private IEnumerable<GrammarDefinition<TContext>> TypeDefinitions()
        {
            return new[]
            {
                new OperandDefinition<TContext>(
                    new Grammar("GUID",
                        @"[0-9A-Fa-f]{8}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{12}"),
                    x => Expression.Constant(Guid.Parse(x.Trim('\'')))),

                new OperandDefinition<TContext>(
                    new Grammar("OBJECTID",
                        @"[a-f\d]{24}"),
                    x => Expression.Constant(x.Trim('\''))),

                new OperandDefinition<TContext>(
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

                new OperandDefinition<TContext>(
                    new Grammar("DECIMAL", @"\-?\d+(\.\d+)?"),
                    x => Expression.Constant(decimal.Parse(x))),

                new OperandDefinition<TContext>(
                    new Grammar("PI", @"[Pp][Ii]"),
                    x => Expression.Constant(Math.PI))
            };
        }

        private IEnumerable<GrammarDefinition<TContext>> LogicalOperatorDefinitions()
        {
            return new GrammarDefinition<TContext>[]
            {
                new BinaryOperatorDefinition<TContext>(
                    new Grammar("EQ", @"\b(eq)\b"),
                    11,
                    ConvertEnumsIfRequired(Expression.Equal)),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("NE", @"\b(ne)\b"),
                    12,
                    ConvertEnumsIfRequired(Expression.NotEqual)),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("GT", @"\b(gt)\b"),
                    13,
                    Expression.GreaterThan),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("GE", @"\b(ge)\b"),
                    14,
                    Expression.GreaterThanOrEqual),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("LT", @"\b(lt)\b"),
                    15,
                    Expression.LessThan),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("LE", @"\b(le)\b"),
                    16,
                    Expression.LessThanOrEqual),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("AND", @"\b(and)\b"),
                    19,
                    Expression.And),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("OR", @"\b(or)\b"),
                    20,
                    Expression.Or),

                new UnaryOperatorDefinition<TContext>(
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

        private IEnumerable<FunctionCallDefinition<TContext>> FunctionDefinitions()
        {
            return new[]
            {
                new FunctionCallDefinition<TContext>(
                    new Grammar("FN_CONVERSION", @"[Ff][Xx][(]"),
                    new[] { typeof(string), typeof(decimal), typeof(IAqlResolver<>) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => AqlFunctions.ApplyConversion<TContext>(null, 0, null)),
                            new[] { parameters[0], parameters[1], Expression.Constant(this) });
                    }),

                new FunctionCallDefinition<TContext>(
                    new Grammar("FN_IF", @"[Ii][Ff][(]"),
                    new[] { typeof(bool), typeof(decimal), typeof(decimal), typeof(IAqlResolver<>) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => AqlFunctions.ApplyIf<TContext>(false, 0, 0, null)), parameters[0],
                            parameters[1], parameters[2], Expression.Constant(this));
                    }),

                new FunctionCallDefinition<TContext>(
                    new Grammar("FN_EACH", @"[Ee][Aa][Cc][Hh][(]"),
                    new[] { typeof(List<decimal>), typeof(string), typeof(IAqlResolver<>) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => AqlFunctions.ApplyEach<TContext>(null, null, null)),
                            new[] { parameters[0], parameters[1], Expression.Constant(this) });
                    }),

                new FunctionCallDefinition<TContext>(
                    new Grammar("FN_SIN", @"[Ss][Ii][Nn]\("),
                    new[] { typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Sin(0)), parameters[0]);
                    }),

                new FunctionCallDefinition<TContext>(
                    new Grammar("FN_COS", @"[Cc][Oo][Ss]\("),
                    new[] { typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Cos(0)), parameters[0]);
                    }),

                new FunctionCallDefinition<TContext>(
                    new Grammar("FN_TAN", @"[Tt][Aa][Nn]\("),
                    new[] { typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Tan(0)), parameters[0]);
                    }),

                new FunctionCallDefinition<TContext>(
                    new Grammar("FN_SQRT", @"[Ss][Qq][Rr][Tt]\("),
                    new[] { typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Sqrt(0)), parameters[0]);
                    }),

                new FunctionCallDefinition<TContext>(new Grammar("FN_POW", @"[Pp][Oo][Ww]\("),
                    new[] { typeof(double), typeof(double) },
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Pow(0, 0)), new[] { parameters[0], parameters[1] });
                    })
            };
        }

        private IEnumerable<GrammarDefinition<TContext>> BracketDefinitions(
            IEnumerable<FunctionCallDefinition<TContext>> functionCalls)
        {
            ParenthesisBracketOpenDefinition<TContext> openParenthesisBracket;
            ListDelimiterDefinition<TContext> delimeter;

            SquareBracketOpenDefinition<TContext> openSquareBracket;
            AngleBracketOpenDefinition<TContext> openAngleBracket;

            return new GrammarDefinition<TContext>[]
            {
                openParenthesisBracket = new ParenthesisBracketOpenDefinition<TContext>(
                    new Grammar("OPEN_PARENTHESIS_BRACKET", @"\(")),

                delimeter = new ListDelimiterDefinition<TContext>(
                    new Grammar("COMMA", ",")),

                new ParenthesisBracketCloseDefinition<TContext>(
                    new Grammar("CLOSE_PARENTHESIS_BRACKET", @"\)"),
                    new[] { openParenthesisBracket }.Concat(functionCalls),
                    delimeter),

                openSquareBracket = new SquareBracketOpenDefinition<TContext>(
                    new Grammar("OPEN_SQUARE_BRACKET", @"\["),
                    this),

                new SquareBracketCloseDefinition<TContext>(
                    new Grammar("CLOSE_SQUARE_BRACKET", @"\]"),
                    new[] { openSquareBracket }, delimeter),

                openAngleBracket = new AngleBracketOpenDefinition<TContext>(
                    new Grammar("OPEN_ANGLE_BRACKET", @"\<"),
                    this),

                new AngleBracketCloseDefinition<TContext>(
                    new Grammar("CLOSE_ANGLE_BRACKET", @"\>"),
                    new[] { openAngleBracket })
            };
        }

        private IEnumerable<GrammarDefinition<TContext>> ArithmeticOperatorDefinitions()
        {
            return new[]
            {
                new BinaryOperatorDefinition<TContext>(
                    new Grammar("ADD", @"\+"),
                    2,
                    Expression.Add),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("SUB", @"\-"),
                    2,
                    Expression.Subtract),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("MUL", @"\*"),
                    1,
                    Expression.Multiply),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("DIV", @"\/"),
                    1,
                    Expression.Divide),

                new BinaryOperatorDefinition<TContext>(
                    new Grammar("MOD", @"%"),
                    1,
                    Expression.Modulo)
            };
        }

        private IEnumerable<GrammarDefinition<TContext>> PropertyDefinitions()
        {
            return new[]
            {
                new OperandDefinition<TContext>(
                    new Grammar("PROPERTY_PATH", @"(?<![0-9])([A-Za-z_][A-Za-z0-9_]*\.?)+"),
                    (value, parameters) =>
                    {
                        return value.Split('.').Aggregate(
                            (Expression)parameters[0],
                            (exp, prop) => Expression.MakeMemberAccess(exp, TypeShim.GetProperty(exp.Type, prop)));
                    })
            };
        }

        private IEnumerable<GrammarDefinition<TContext>> WhitespaceDefinitions()
        {
            return new[]
            {
                new WhitespaceDefinition<TContext>(new Grammar("WHITESPACE", @"\s+", true))
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
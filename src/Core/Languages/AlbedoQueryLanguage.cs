namespace AlbedoTeam.Sdk.QueryLanguage.Core.Languages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Definitions;
    using Definitions.Brackets;
    using Injections;
    using Structs;
    using Utility;

    public class InnerDep
    {
        public InnerDep(AqlFormula aqlFormula, ResolverResponse response)
        {
            Formula = aqlFormula;
            Resolver = response;
        }

        public AqlFormula Formula { get; set; }
        public ResolverResponse Resolver { get; set; }
    }

    public class AqlFormula
    {
        public AqlFormula()
        {
            InnerOps = new List<Operand>();
        }

        public string ExpressionText { get; set; }
        public string Raw { get; set; }
        public decimal Result { get; set; }

        public List<Operand> InnerOps { get; set; }
    }

    public class AlbedoQueryLanguage
    {
        private readonly List<AqlFormula> _instanceFormulas = new List<AqlFormula>();
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
            body = ExpressionConversions.Convert(body, typeof(decimal));

            var expressionFunction = Expression.Lambda<Func<decimal>>(body);
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

        private static string GetRawDisplay(Operand op)
        {
            if (op.Expression is ConstantExpression)
            {
                if (!op.Expression.ToString().Contains("${")) return "";
            }
            else
            {
                const string resolverExp = ", value(AlbedoTeam.Sdk.QueryLanguage.Core.Languages.AlbedoQueryLanguage)";
                if (op.Expression.ToString().Contains(resolverExp))
                    // return $"{Deparse(op.Expression.ToString())}";
                    return $"{op.Expression.ToString().Replace(resolverExp, "")}";
            }

            return $"{op.Expression}";
        }

        // private static string Deparse(string value)
        // {
        //     const string commaPattern = "[^,\\s][^\\,]*[^,\\s]*";
        //     var commaReg = new Regex(commaPattern);
        //     var commaMatches = commaReg.Matches(value);
        //
        //     if (commaMatches.Count != 3)
        //         throw new OperandExpectedException(new StringSegment("Bad groups", 0, 0));
        //
        //     const string quotePattern = "\"[^\"]*\"";
        //
        //     var quoteReg = new Regex(quotePattern);
        //     var quoteMatches = quoteReg.Matches(commaMatches[0].Value);
        //     if (quoteMatches.Count != 1)
        //         throw new OperandExpectedException(new StringSegment("Bad input", 0, 0));
        //
        //     var baseFormula = quoteMatches[0].Value.Replace("\"", "");
        //     var input = commaMatches[1].Value;
        //
        //     return baseFormula.Replace("${input}", input);
        // }

        // public Expression<Func<T, decimal>> Parse<T>(string text)
        // {
        //     var parameters = new[] {Expression.Parameter(typeof(T))};
        //     var (body, parsingState) = _language.Parse(text, parameters);
        //     body = ExpressionConversions.Convert(body, typeof(decimal));
        //     return Expression.Lambda<Func<T, decimal>>(body, parameters);
        // }

        protected virtual IEnumerable<GrammarDefinition> AllDefinitions()
        {
            IEnumerable<FunctionCallDefinition> functions;
            var definitions = new List<GrammarDefinition>();

            definitions.AddRange(TypeDefinitions());
            definitions.AddRange(functions = FunctionDefinitions());
            definitions.AddRange(BracketDefinitions(functions));
            definitions.AddRange(ArithmeticOperatorDefinitions());
            definitions.AddRange(PropertyDefinitions());
            definitions.AddRange(WhitespaceDefinitions());
            return definitions;
        }

        protected virtual IEnumerable<GrammarDefinition> TypeDefinitions()
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
                    new Grammar("DECIMAL", @"\-?\d+(\.\d+)?"),
                    x => Expression.Constant(decimal.Parse(x))),

                new OperandDefinition(
                    new Grammar("PI", @"[Pp][Ii]"),
                    x => Expression.Constant(Math.PI))
            };
        }

        protected virtual IEnumerable<FunctionCallDefinition> FunctionDefinitions()
        {
            return new[]
            {
                new FunctionCallDefinition(
                    new Grammar("FN_CONVERSION", @"[Ff][Xx][(]"),
                    new[] {typeof(string), typeof(decimal), typeof(IAqlResolver)},
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => AqlFunctions.Apply(null, 0, null)),
                            new[] {parameters[0], parameters[1], Expression.Constant(this)});
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_SIN", @"[Ss][Ii][Nn]\("),
                    new[] {typeof(double)},
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Sin(0)), parameters[0]);
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_COS", @"[Cc][Oo][Ss]\("),
                    new[] {typeof(double)},
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Cos(0)), parameters[0]);
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_TAN", @"[Tt][Aa][Nn]\("),
                    new[] {typeof(double)},
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Tan(0)), parameters[0]);
                    }),

                new FunctionCallDefinition(
                    new Grammar("FN_SQRT", @"[Ss][Qq][Rr][Tt]\("),
                    new[] {typeof(double)},
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Sqrt(0)), parameters[0]);
                    }),

                new FunctionCallDefinition(new Grammar("FN_POW", @"[Pp][Oo][Ww]\("),
                    new[] {typeof(double), typeof(double)},
                    parameters =>
                    {
                        return Expression.Call(
                            null,
                            Type<object>.Method(x => Math.Pow(0, 0)), new[] {parameters[0], parameters[1]});
                    })
            };
        }

        protected virtual IEnumerable<GrammarDefinition> BracketDefinitions(
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
                    new[] {openParenthesisBracket}.Concat(functionCalls),
                    delimeter),

                openSquareBracket = new SquareBracketOpenDefinition(
                    new Grammar("OPEN_SQUARE_BRACKET", @"\["),
                    this),

                new SquareBracketCloseDefinition(
                    new Grammar("CLOSE_SQUARE_BRACKET", @"\]"),
                    new[] {openSquareBracket}),

                openAngleBracket = new AngleBracketOpenDefinition(
                    new Grammar("OPEN_ANGLE_BRACKET", @"\<"),
                    this),

                new AngleBracketCloseDefinition(
                    new Grammar("CLOSE_ANGLE_BRACKET", @"\>"),
                    new[] {openAngleBracket})
            };
        }

        protected virtual IEnumerable<GrammarDefinition> ArithmeticOperatorDefinitions()
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

        protected virtual IEnumerable<GrammarDefinition> PropertyDefinitions()
        {
            return new[]
            {
                new OperandDefinition(
                    new Grammar("PROPERTY_PATH", @"(?<![0-9])([A-Za-z_][A-Za-z0-9_]*\.?)+"),
                    (value, parameters) =>
                    {
                        return value.Split('.').Aggregate(
                            (Expression) parameters[0],
                            (exp, prop) => Expression.MakeMemberAccess(exp, TypeShim.GetProperty(exp.Type, prop)));
                    })
            };
        }

        protected virtual IEnumerable<GrammarDefinition> WhitespaceDefinitions()
        {
            return new[]
            {
                new WhitespaceDefinition(new Grammar("WHITESPACE", @"\s+", true))
            };
        }
    }
}
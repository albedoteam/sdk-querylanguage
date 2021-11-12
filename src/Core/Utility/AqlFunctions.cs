namespace AlbedoTeam.Sdk.QueryLanguage.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using Injections;
    using Languages;

    public static class AqlFunctions
    {
        public static decimal ApplyConversion<TContext>(
            string formula,
            decimal value,
            AlbedoQueryLanguage<TContext> language)
            where TContext : IResolverContext
        {
            // fx(<60770b574d0bf5d86a69332c>, 3)
            // {x} * [60770b6a4d0bf5d86a693311]

            formula = formula.Replace("{x}", value.ToString(CultureInfo.InvariantCulture));
            var innerExpression = Resolve(formula, language);

            var le = Expression.Lambda<Func<decimal>>(innerExpression);
            var compiledExpression = le.Compile();
            var result = compiledExpression();
            return result;
        }

        public static decimal ApplyIf<TContext>(
            bool condition,
            decimal trueValue,
            decimal falseValue,
            AlbedoQueryLanguage<TContext> language)
            where TContext : IResolverContext
        {
            // if((1 gt 2) and (2 gt 3), 1, 2)
            // if(1 gt 2, 1, 2)
            // if([condition], [true_value], [false_value])

            return condition ? trueValue : falseValue;
        }

        public static decimal ApplyEach<TContext>(
            IEnumerable<decimal> inputValues,
            string formula,
            AlbedoQueryLanguage<TContext> language)
            where TContext : IResolverContext
        {
            // each([inputId], fx(<[formulaId]>, inputValue))

            var results = inputValues
                .Select(value => ApplyConversion(formula, value, language))
                .ToList();

            // for now, return the sum
            return results.Sum();
        }

        private static ConstantExpression Resolve<TContext>(
            string formula,
            AlbedoQueryLanguage<TContext> language)
            where TContext : IResolverContext
        {
            var parseResponse = language.Parse(new ParseRequest<TContext>
            {
                Context = language.Context,
                Formula = formula
            });

            var innerExpressionResult = Expression.Constant(parseResponse.Result);

            return innerExpressionResult;
        }
    }
}
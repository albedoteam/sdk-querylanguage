namespace AlbedoTeam.Sdk.QueryLanguage.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using Languages;

    public static class AqlFunctions
    {
        public static decimal ApplyConversion(string formula, decimal value, AlbedoQueryLanguage language)
        {
            // fx(<60770b574d0bf5d86a69332c>, 3)
            // ${input} * [60770b6a4d0bf5d86a693311]

            formula = formula.Replace("${input}", value.ToString(CultureInfo.InvariantCulture));
            var (innerExpression, innerDep) = Resolve(formula, language);

            var le = Expression.Lambda<Func<decimal>>(innerExpression);
            var compiledExpression = le.Compile();
            var result = compiledExpression();
            return result;
        }

        public static decimal ApplyIf(bool condition, decimal trueValue, decimal falseValue,
            AlbedoQueryLanguage language)
        {
            // if((1 gt 2) and (2 gt 3), 1, 2)
            // if(1 gt 2, 1, 2)
            // if([condition], [true_value], [false_value])

            return condition ? trueValue : falseValue;
        }

        public static decimal ApplyEach(IEnumerable<decimal> inputValues, string formula, AlbedoQueryLanguage language)
        {
            // each([inputId], fx(<[formulaId]>, inputValue))

            var results = inputValues
                .Select(value => ApplyConversion(formula, value, language))
                .ToList();

            // for now, return the sum
            return results.Sum();
        }

        private static (ConstantExpression, InnerDep) Resolve(string formula, AlbedoQueryLanguage language)
        {
            var aqlFormula = language.Parse(formula);
            var innerExpressionResult = Expression.Constant(aqlFormula.Result);

            return (innerExpressionResult, new InnerDep(aqlFormula, null));
        }
    }
}
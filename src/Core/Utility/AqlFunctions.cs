namespace AlbedoTeam.Sdk.QueryLanguage.Core.Utility
{
    using System;
    using System.Globalization;
    using System.Linq.Expressions;
    using Languages;

    public static class AqlFunctions
    {
        public static decimal Apply(string formula, decimal value, AlbedoQueryLanguage language)
        {
            formula = formula.Replace("${input}", value.ToString(CultureInfo.InvariantCulture));
            var (innerExpression, innerDep) = Resolve(formula, language);

            var le = Expression.Lambda<Func<decimal>>(innerExpression);
            var compiledExpression = le.Compile();
            var result = compiledExpression();
            return result;
        }

        private static (ConstantExpression, InnerDep) Resolve(string formula, AlbedoQueryLanguage language)
        {
            var aqlFormula = language.Parse(formula);
            var innerExpressionResult = Expression.Constant(aqlFormula.Result);

            return (innerExpressionResult, new InnerDep(aqlFormula, null));
        }
    }
}
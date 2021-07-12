namespace AlbedoTeam.Sdk.QueryLanguage.Definitions
{
    using System;
    using System.Linq.Expressions;
    using Core.Enumerators;
    using Core.Structs;
    using Core.Utility;

    public class BinaryOperatorDefinition : OperatorDefinition
    {
        private static readonly RelativePosition[] LeftRight = {RelativePosition.Left, RelativePosition.Right};

        public BinaryOperatorDefinition(
            Grammar grammar,
            int orderOfPrecedence,
            Func<Expression, Expression, Expression> expressionBuilder)
            : base(
                grammar,
                orderOfPrecedence,
                LeftRight,
                param =>
                {
                    var left = param[0];
                    var right = param[1];
                    ExpressionConversions.TryImplicitlyConvert(ref left, ref right);
                    return expressionBuilder(left, right);
                })
        {
        }
    }
}
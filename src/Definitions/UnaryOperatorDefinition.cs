namespace AlbedoTeam.Sdk.QueryLanguage.Definitions
{
    using System;
    using System.Linq.Expressions;
    using Core.Enumerators;
    using Core.Structs;

    /// <summary>
    ///     Represents an operator that takes a single operand
    /// </summary>
    /// <seealso cref="OperatorDefinition" />
    internal class UnaryOperatorDefinition : OperatorDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnaryOperatorDefinition" /> class.
        /// </summary>
        /// <param name="grammar">The grammar of the definition</param>
        /// <param name="orderOfPrecedence">The relative order this operator should be applied. Lower orders are applied first</param>
        /// <param name="operandPosition">The relative positions where the single operand can be found</param>
        /// <param name="expressionBuilder">The function given the expressions of single operand, produces a new operand</param>
        public UnaryOperatorDefinition(
            Grammar grammar,
            int orderOfPrecedence,
            RelativePosition operandPosition,
            Func<Expression, Expression> expressionBuilder)
            : base(grammar, orderOfPrecedence, new[] {operandPosition}, param => expressionBuilder(param[0]))
        {
        }
    }
}
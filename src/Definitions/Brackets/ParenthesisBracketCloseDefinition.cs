namespace AlbedoTeam.Sdk.QueryLanguage.Definitions.Brackets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.States;
    using Core.Structs;
    using Exceptions;
    using Injections;

    public class ParenthesisBracketCloseDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        public readonly IReadOnlyCollection<ParenthesisBracketOpenDefinition<TContext>> BracketOpenDefinitions;
        public readonly GrammarDefinition<TContext> ListDelimeterDefinition;

        public ParenthesisBracketCloseDefinition(
            Grammar grammar,
            IEnumerable<ParenthesisBracketOpenDefinition<TContext>> bracketOpenDefinitions,
            GrammarDefinition<TContext> listDelimeterDefinition = null) : base(grammar)
        {
            if (bracketOpenDefinitions == null)
                throw new ArgumentNullException(nameof(bracketOpenDefinitions));

            BracketOpenDefinitions = bracketOpenDefinitions.ToList();
            ListDelimeterDefinition = listDelimeterDefinition;
        }

        public ParenthesisBracketCloseDefinition(
            Grammar grammar,
            ParenthesisBracketOpenDefinition<TContext> bracketOpenDefinition,
            GrammarDefinition<TContext> listDelimeterDefinition = null)
            : this(grammar, new[] { bracketOpenDefinition }, listDelimeterDefinition)
        {
        }

        public override void Apply(Token<TContext> token, ParsingState<TContext> state)
        {
            var bracketOperands = new Stack<Operand>();
            var previousSeperator = token.StringSegment;
            var hasSeperators = false;

            while (state.Operators.Count > 0)
            {
                var currentOperator = state.Operators.Pop();
                if (BracketOpenDefinitions.Contains(currentOperator.Definition))
                {
                    var operand = state.Operands.Count > 0 ? state.Operands.Peek() : (Operand?)null;
                    var firstSegment = currentOperator.StringSegment;
                    var secondSegment = previousSeperator;

                    if (operand != null && operand.Value.StringSegment.IsBetween(firstSegment, secondSegment))
                        bracketOperands.Push(state.Operands.Pop());

                    else if (hasSeperators && (operand == null ||
                                               !operand.Value.StringSegment.IsBetween(firstSegment, secondSegment)))
                        // if we have separators, then we should have something between the last separator and the open bracket
                        throw new OperandExpectedException(StringSegment.Between(firstSegment, secondSegment));

                    // pass all of our bracket operands to the bracket method, it will know what to do
                    var closeBracketOperator = new Operator<TContext>(this, token.StringSegment, () => { });

                    ((ParenthesisBracketOpenDefinition<TContext>)currentOperator.Definition).ApplyBracketOperands(
                        currentOperator,
                        bracketOperands,
                        closeBracketOperator,
                        state);

                    return;
                }

                if (ListDelimeterDefinition != null && currentOperator.Definition == ListDelimeterDefinition)
                {
                    hasSeperators = true;
                    var operand = state.Operands.Pop();

                    // if our operator is not between two delimiters, an operator is missing
                    var firstSegment = currentOperator.StringSegment;
                    var secondSegment = previousSeperator;
                    if (!operand.StringSegment.IsBetween(firstSegment, secondSegment))
                        throw new OperandExpectedException(StringSegment.Between(firstSegment, secondSegment));

                    bracketOperands.Push(operand);
                    previousSeperator = currentOperator.StringSegment;
                }
                else
                {
                    // regular operator, execute it
                    currentOperator.Execute();
                }
            }

            // we went through all the operators and didn’t find an open bracket
            throw new BracketUnmatchedException(token.StringSegment);
        }
    }
}
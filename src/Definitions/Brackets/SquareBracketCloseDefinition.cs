namespace AlbedoTeam.Sdk.QueryLanguage.Definitions.Brackets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.States;
    using Core.Structs;
    using Exceptions;

    public class SquareBracketCloseDefinition : GrammarDefinition
    {
        public readonly IReadOnlyCollection<SquareBracketOpenDefinition> BracketOpenDefinitions;

        public SquareBracketCloseDefinition(
            Grammar grammar,
            IEnumerable<SquareBracketOpenDefinition> bracketOpenDefinitions)
            : base(grammar)
        {
            if (bracketOpenDefinitions == null)
                throw new ArgumentNullException(nameof(bracketOpenDefinitions));

            BracketOpenDefinitions = bracketOpenDefinitions.ToList();
        }

        public override void Apply(Token token, ParsingState state)
        {
            var bracketOperands = new Stack<Operand>();
            var previousSeperator = token.StringSegment;

            while (state.Operators.Count > 0)
            {
                var currentOperator = state.Operators.Pop();
                if (BracketOpenDefinitions.Contains(currentOperator.Definition))
                {
                    var operand = state.Operands.Count > 0 ? state.Operands.Peek() : (Operand?) null;
                    var firstSegment = currentOperator.StringSegment;

                    if (operand != null && operand.Value.StringSegment.IsBetween(firstSegment, previousSeperator))
                        bracketOperands.Push(state.Operands.Pop());

                    var closeBracketOperator = new Operator(this, token.StringSegment, () => { });

                    ((SquareBracketOpenDefinition) currentOperator.Definition).ApplyBracketOperands(
                        currentOperator,
                        bracketOperands,
                        closeBracketOperator,
                        state);

                    return;
                }

                currentOperator.Execute();
            }

            throw new BracketUnmatchedException(token.StringSegment);
        }
    }
}
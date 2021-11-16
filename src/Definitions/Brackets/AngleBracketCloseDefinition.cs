namespace AlbedoTeam.Sdk.QueryLanguage.Definitions.Brackets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.States;
    using Core.Structs;
    using Exceptions;
    using Injections;

    public class AngleBracketCloseDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        public readonly IReadOnlyCollection<AngleBracketOpenDefinition<TContext>> BracketOpenDefinitions;

        public AngleBracketCloseDefinition(
            Grammar grammar,
            IEnumerable<AngleBracketOpenDefinition<TContext>> bracketOpenDefinitions)
            : base(grammar)
        {
            if (bracketOpenDefinitions == null)
                throw new ArgumentNullException(nameof(bracketOpenDefinitions));

            BracketOpenDefinitions = bracketOpenDefinitions.ToList();
        }

        public override void Apply(Token<TContext> token, ParsingState<TContext> state)
        {
            var bracketOperands = new Stack<Operand>();
            var previousSeperator = token.StringSegment;

            while (state.Operators.Count > 0)
            {
                var currentOperator = state.Operators.Pop();
                if (BracketOpenDefinitions.Contains(currentOperator.Definition))
                {
                    var operand = state.Operands.Count > 0 ? state.Operands.Peek() : (Operand?)null;
                    var firstSegment = currentOperator.StringSegment;

                    if (operand != null && operand.Value.StringSegment.IsBetween(firstSegment, previousSeperator))
                        bracketOperands.Push(state.Operands.Pop());

                    var closeBracketOperator = new Operator<TContext>(this, token.StringSegment, () => { });

                    ((AngleBracketOpenDefinition<TContext>)currentOperator.Definition).ApplyBracketOperands(
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
namespace AlbedoTeam.Sdk.QueryLanguage.Definitions.Brackets
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.States;
    using Core.Structs;
    using Exceptions;
    using Injections;

    public class ParenthesisBracketOpenDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        public ParenthesisBracketOpenDefinition(Grammar grammar) : base(grammar)
        {
        }

        public override void Apply(Token<TContext> token, ParsingState<TContext> state)
        {
            state.Operators.Push(new Operator<TContext>(
                this,
                token.StringSegment,
                () => throw new BracketUnmatchedException(token.StringSegment)));
        }

        public virtual void ApplyBracketOperands(
            Operator<TContext> bracketOpen,
            Stack<Operand> bracketOperands,
            Operator<TContext> bracketClose,
            ParsingState<TContext> state)
        {
            if (bracketOperands.Count == 0)
            {
                var insideBrackets = StringSegment.Between(bracketOpen.StringSegment, bracketClose.StringSegment);
                throw new OperandExpectedException(insideBrackets);
            }

            if (bracketOperands.Count > 1)
            {
                var operandSpan = StringSegment.Encompass(bracketOperands.Skip(1).Select(x => x.StringSegment));
                throw new OperandUnexpectedException(operandSpan);
            }

            var bracketOperand = bracketOperands.Pop();

            var sourceMap = StringSegment.Encompass(
                bracketOpen.StringSegment,
                bracketOperand.StringSegment,
                bracketClose.StringSegment);

            state.Operands.Push(new Operand(bracketOperand.Expression, sourceMap));
        }
    }
}
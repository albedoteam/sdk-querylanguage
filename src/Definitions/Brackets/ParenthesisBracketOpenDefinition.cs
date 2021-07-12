namespace AlbedoTeam.Sdk.QueryLanguage.Definitions.Brackets
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.States;
    using Core.Structs;
    using Exceptions;

    public class ParenthesisBracketOpenDefinition : GrammarDefinition
    {
        public ParenthesisBracketOpenDefinition(Grammar grammar) : base(grammar)
        {
        }

        public override void Apply(Token token, ParsingState state)
        {
            state.Operators.Push(new Operator(
                this,
                token.StringSegment,
                () => throw new BracketUnmatchedException(token.StringSegment)));
        }

        public virtual void ApplyBracketOperands(
            Operator bracketOpen,
            Stack<Operand> bracketOperands,
            Operator bracketClose,
            ParsingState state)
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
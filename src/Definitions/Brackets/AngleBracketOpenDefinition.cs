namespace AlbedoTeam.Sdk.QueryLanguage.Definitions.Brackets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Core.Languages;
    using Core.States;
    using Core.Structs;
    using Exceptions;
    using Injections;

    public class AngleBracketOpenDefinition : GrammarDefinition
    {
        private readonly AlbedoQueryLanguage _language;

        public AngleBracketOpenDefinition(Grammar grammar, AlbedoQueryLanguage language) : base(grammar)
        {
            _language = language;
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
            var (expression, innerDep) = Resolve(bracketOpen, bracketOperand, bracketClose);

            var sourceMap = StringSegment.Encompass(
                bracketOpen.StringSegment,
                bracketOperand.StringSegment,
                bracketClose.StringSegment);

            state.Operands.Push(new Operand(expression, sourceMap, innerDep));
        }

        private (ConstantExpression, InnerDep) Resolve(
            Operator bracketOpen,
            Operand bracketOperand,
            Operator bracketClose)
        {
            var le = Expression.Lambda<Func<string>>(bracketOperand.Expression);
            var compiledExpression = le.Compile();
            var idToBeResolved = compiledExpression();

            var response = _language.Resolver.ReferenceResolver(new ResolverRequest
            {
                InputId = idToBeResolved,
                InputType = InputType.Function
            }).Result;

            if (response.Values[0].Contains("${"))
                return (Expression.Constant(response.Values[0]), new InnerDep(null, response));

            var insideBrackets = StringSegment.Between(bracketOpen.StringSegment, bracketClose.StringSegment);
            throw new OperandExpectedException(insideBrackets);
        }
    }
}
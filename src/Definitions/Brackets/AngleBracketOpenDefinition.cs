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

    public class AngleBracketOpenDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        private readonly AlbedoQueryLanguage<TContext> _language;

        public AngleBracketOpenDefinition(Grammar grammar, AlbedoQueryLanguage<TContext> language) : base(grammar)
        {
            _language = language;
        }

        public override void Apply(Token<TContext> token, ParsingState<TContext> state)
        {
            state.Request = token.Request;
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
            var expression = Resolve(bracketOpen, bracketOperand, bracketClose, state.Request);

            var sourceMap = StringSegment.Encompass(
                bracketOpen.StringSegment,
                bracketOperand.StringSegment,
                bracketClose.StringSegment);

            state.Operands.Push(new Operand(expression, sourceMap));
        }

        private ConstantExpression Resolve(
            Operator<TContext> bracketOpen,
            Operand bracketOperand,
            Operator<TContext> bracketClose,
            ParseRequest<TContext> request)
        {
            var le = Expression.Lambda<Func<string>>(bracketOperand.Expression);
            var compiledExpression = le.Compile();
            var idToBeResolved = compiledExpression();

            var response = _language.Resolver.ReferenceResolver(_language, new ResolverRequest<TContext>
            {
                Context = request.Context,
                ReferenceId = idToBeResolved,
                ReferenceType = ReferenceType.Function
            }).Result;

            if (response.ResolverResults is { } && response.ResolverResults.Count > 0)
                if (response.ResolverResults[0].Value.Contains("${"))
                    return Expression.Constant(response.ResolverResults[0].Value);

            var insideBrackets = StringSegment.Between(bracketOpen.StringSegment, bracketClose.StringSegment);
            throw new OperandExpectedException(insideBrackets);
        }
    }
}
namespace AlbedoTeam.Sdk.QueryLanguage.Definitions
{
    using System;
    using System.Linq.Expressions;
    using Core.States;
    using Core.Structs;
    using Exceptions;
    using Injections;

    public class OperandDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        public readonly Func<string, ParameterExpression[], Expression> ExpressionBuilder;

        public OperandDefinition(
            Grammar grammar,
            Func<string, Expression> expressionBuilder) :
            this(grammar, (v, a) => expressionBuilder(v))
        {
        }

        public OperandDefinition(
            Grammar grammar,
            Func<string, ParameterExpression[], Expression> expressionBuilder)
            : base(grammar)
        {
            ExpressionBuilder = expressionBuilder ?? throw new ArgumentNullException(nameof(expressionBuilder));
        }

        public override void Apply(Token<TContext> token, ParsingState<TContext> state)
        {
            Expression expression;
            try
            {
                expression = ExpressionBuilder(token.Value, state.Parameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new OperationInvalidException(token.StringSegment, ex);
            }

            state.Operands.Push(new Operand(expression, token.StringSegment));
        }
    }
}
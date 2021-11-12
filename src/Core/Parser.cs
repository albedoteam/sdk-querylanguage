namespace AlbedoTeam.Sdk.QueryLanguage.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Exceptions;
    using Injections;
    using States;
    using Structs;

    internal class Parser<TContext>
        where TContext : IResolverContext
    {
        public Expression Parse(IEnumerable<Token<TContext>> tokens, IEnumerable<ParameterExpression> parameters = null)
        {
            parameters ??= Enumerable.Empty<ParameterExpression>();

            var compileState = new ParsingState<TContext>();
            compileState.Parameters.AddRange(parameters);

            foreach (var token in tokens) token.Definition.Apply(token, compileState);

            var outputExpression = FoldOperators(compileState);

            return outputExpression;
        }

        private static Expression FoldOperators(ParsingState<TContext> state)
        {
            while (state.Operators.Count > 0)
            {
                var @operator = state.Operators.Pop();
                @operator.Execute();
            }

            if (state.Operands.Count == 0)
                throw new OperandExpectedException(new StringSegment("", 0, 0));

            if (state.Operands.Count > 1)
                throw new OperandUnexpectedException(state.Operands.Peek().StringSegment);

            return state.Operands.Peek().Expression;
        }
    }
}
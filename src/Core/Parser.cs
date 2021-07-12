namespace AlbedoTeam.Sdk.QueryLanguage.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Exceptions;
    using States;
    using Structs;

    internal class Parser
    {
        public (Expression, Dictionary<string, Operand>) Parse(
            IEnumerable<Token> tokens,
            IEnumerable<ParameterExpression> parameters = null)
        {
            var operandsDictionary = new Dictionary<string, Operand>();
            parameters ??= Enumerable.Empty<ParameterExpression>();

            var compileState = new ParsingState();
            compileState.Parameters.AddRange(parameters);

            foreach (var token in tokens)
            {
                foreach (var operand in compileState.Operands.ToList())
                    operandsDictionary.TryAdd(operand.StringSegment.ToString(), operand);

                token.Definition.Apply(token, compileState);

                foreach (var operand in compileState.Operands.ToList())
                    operandsDictionary.TryAdd(operand.StringSegment.ToString(), operand);
            }

            var outputExpression = FoldOperators(compileState);

            return (outputExpression, operandsDictionary);
        }

        private static Expression FoldOperators(ParsingState state)
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
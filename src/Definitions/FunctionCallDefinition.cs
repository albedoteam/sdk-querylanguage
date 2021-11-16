namespace AlbedoTeam.Sdk.QueryLanguage.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Brackets;
    using Core.States;
    using Core.Structs;
    using Core.Utility;
    using Exceptions;
    using Injections;

    public class FunctionCallDefinition<TContext> : ParenthesisBracketOpenDefinition<TContext>
        where TContext : IResolverContext
    {
        public readonly IReadOnlyList<Type> ArgumentTypes;
        public readonly Func<Expression[], Expression> ExpressionBuilder;

        public FunctionCallDefinition(
            Grammar grammar,
            IEnumerable<Type> argumentTypes,
            Func<Expression[], Expression> expressionBuilder)
            : base(grammar)
        {
            ArgumentTypes = argumentTypes?.ToList();
            ExpressionBuilder = expressionBuilder;
        }

        public FunctionCallDefinition(Grammar grammar, Func<Expression[], Expression> expressionBuilder)
            : this(grammar, null, expressionBuilder)
        {
        }

        public override void ApplyBracketOperands(
            Operator<TContext> bracketOpen,
            Stack<Operand> bracketOperands,
            Operator<TContext> bracketClose, ParsingState<TContext> state)
        {
            var operandSource = StringSegment.Encompass(bracketOperands.Select(x => x.StringSegment));
            var functionArguments = bracketOperands.Select(x => x.Expression);

            if (ArgumentTypes != null)
            {
                var argCount = ArgumentTypes.Count;
                if (ArgumentTypes.Contains(typeof(IAqlResolver<>)))
                    argCount--;

                var expectedArgumentCount = argCount;
                if (expectedArgumentCount != bracketOperands.Count)
                    throw new FunctionArgumentCountException(
                        operandSource,
                        expectedArgumentCount,
                        bracketOperands.Count);

                functionArguments = bracketOperands.Zip(ArgumentTypes, (o, t) =>
                {
                    try
                    {
                        return ExpressionConversions.Convert(o.Expression, t);
                    }
                    catch (InvalidOperationException)
                    {
                        throw new FunctionArgumentTypeException(o.StringSegment, t, o.Expression.Type);
                    }
                });
            }

            var functionSourceMap = StringSegment.Encompass(bracketOpen.StringSegment, operandSource);
            var functionArgumentsArray = functionArguments.ToArray();

            Expression output;
            try
            {
                output = ExpressionBuilder(functionArgumentsArray);
            }
            catch (Exception ex)
            {
                throw new OperationInvalidException(functionSourceMap, ex);
            }

            if (output != null)
                state.Operands.Push(new Operand(output, functionSourceMap));
        }
    }
}
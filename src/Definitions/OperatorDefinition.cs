namespace AlbedoTeam.Sdk.QueryLanguage.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Core.Enumerators;
    using Core.Extensions;
    using Core.States;
    using Core.Structs;
    using Exceptions;
    using Injections;

    public class OperatorDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        public readonly Func<Expression[], Expression> ExpressionBuilder;
        public readonly int? OrderOfPrecedence;
        public readonly IReadOnlyList<RelativePosition> ParamaterPositions;

        public OperatorDefinition(
            Grammar grammar,
            int? orderOfPrecedence,
            IEnumerable<RelativePosition> paramaterPositions,
            Func<Expression[], Expression> expressionBuilder)
            : base(grammar)
        {
            if (paramaterPositions == null)
                throw new ArgumentNullException(nameof(paramaterPositions));

            ParamaterPositions = paramaterPositions.ToList();
            ExpressionBuilder = expressionBuilder ?? throw new ArgumentNullException(nameof(expressionBuilder));
            OrderOfPrecedence = orderOfPrecedence;
        }

        public OperatorDefinition(
            Grammar grammar,
            IEnumerable<RelativePosition> paramaterPositions,
            Func<Expression[], Expression> expressionBuilder)
            : this(grammar, null, paramaterPositions, expressionBuilder)
        {
        }


        public override void Apply(Token<TContext> token, ParsingState<TContext> state)
        {
            var anyLeftOperators = ParamaterPositions.Any(x => x == RelativePosition.Left);
            while (state.Operators.Count > 0 && OrderOfPrecedence != null && anyLeftOperators)
            {
                var prevOperator = state.Operators.Peek().Definition as OperatorDefinition<TContext>;
                var prevOperatorPrecedence = prevOperator?.OrderOfPrecedence;
                if (prevOperatorPrecedence <= OrderOfPrecedence &&
                    prevOperator.ParamaterPositions.Any(x => x == RelativePosition.Right))
                    state.Operators.Pop().Execute();
                else
                    break;
            }

            state.Operators.Push(new Operator<TContext>(this, token.StringSegment, () =>
            {
                var rightArgs =
                    new Stack<Operand>(state.Operands.PopWhile(x => x.StringSegment.IsRightOf(token.StringSegment)));
                var expectedRightArgs = ParamaterPositions.Count(x => x == RelativePosition.Right);
                if (expectedRightArgs > 0 && rightArgs.Count > expectedRightArgs)
                {
                    var spanWhereOperatorExpected = StringSegment.Encompass(rightArgs
                        .Reverse()
                        .Take(rightArgs.Count - expectedRightArgs)
                        .Select(x => x.StringSegment));

                    throw new OperandUnexpectedException(token.StringSegment, spanWhereOperatorExpected);
                }

                if (rightArgs.Count < expectedRightArgs)
                    throw new OperandExpectedException(
                        token.StringSegment,
                        new StringSegment(token.StringSegment.SourceString, token.StringSegment.End, 0));


                var nextOperatorEndIndex = state.Operators.Count == 0 ? 0 : state.Operators.Peek().StringSegment.End;
                var expectedLeftArgs = ParamaterPositions.Count(x => x == RelativePosition.Left);

                var leftArgs = new Stack<Operand>(state.Operands
                    .PopWhile((x, i) => i < expectedLeftArgs && x.StringSegment.IsRightOf(nextOperatorEndIndex)));

                if (leftArgs.Count < expectedLeftArgs)
                    throw new OperandExpectedException(
                        token.StringSegment,
                        new StringSegment(token.StringSegment.SourceString, token.StringSegment.Start, 0));

                var args = ParamaterPositions.Select(paramPos => paramPos == RelativePosition.Right
                        ? rightArgs.Pop()
                        : leftArgs.Pop())
                    .ToList();


                var sourceMapSpan =
                    StringSegment.Encompass(new[] { token.StringSegment }.Concat(args.Select(x => x.StringSegment)));

                Expression expression;
                try
                {
                    expression = ExpressionBuilder(args.Select(x => x.Expression).ToArray());
                }
                catch (Exception ex)
                {
                    throw new OperationInvalidException(sourceMapSpan, ex);
                }

                state.Operands.Push(new Operand(expression, sourceMapSpan));
            }));
        }
    }
}
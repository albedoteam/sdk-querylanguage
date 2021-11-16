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

    public class SquareBracketOpenDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        private readonly AlbedoQueryLanguage<TContext> _language;

        public SquareBracketOpenDefinition(Grammar grammar, AlbedoQueryLanguage<TContext> language) : base(grammar)
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

            if (bracketOperands.Count > 2)
            {
                var operandSpan = StringSegment.Encompass(bracketOperands.Skip(1).Select(x => x.StringSegment));
                throw new OperandUnexpectedException(operandSpan);
            }

            if (bracketOperands.Count == 2)
            {
                // table reference
                var expression = ResolveCell(bracketOperands, state.Request);

                var bracketOperand = bracketOperands.Pop();

                var sourceMap = StringSegment.Encompass(
                    bracketOpen.StringSegment,
                    bracketOperand.StringSegment,
                    bracketClose.StringSegment);

                state.Operands.Push(new Operand(expression, sourceMap));
            }
            else
            {
                var bracketOperand = bracketOperands.Pop();
                var expression = Resolve(bracketOperand, state.Request);

                var sourceMap = StringSegment.Encompass(
                    bracketOpen.StringSegment,
                    bracketOperand.StringSegment,
                    bracketClose.StringSegment);

                state.Operands.Push(new Operand(expression, sourceMap));
            }
        }

        private ConstantExpression Resolve(Operand bracketOperand, ParseRequest<TContext> request)
        {
            var le = Expression.Lambda<Func<string>>(bracketOperand.Expression);
            var compiledExpression = le.Compile();
            var idToBeResolved = compiledExpression();

            var resolverResponse = _language.Resolver.ReferenceResolver(_language, new ResolverRequest<TContext>
            {
                Context = request.Context,
                ReferenceId = idToBeResolved,
                ReferenceType = ReferenceType.DataInput,
                PreviousResponse = request.PreviousResponse
            }).Result;

            if (resolverResponse.ResolverResults is { } && resolverResponse.ResolverResults.Count > 0)
            {
                if (resolverResponse.ResolverResults.Count == 1 && !resolverResponse.ReferenceAllowsMultipleValues)
                {
                    var parseResponse = _language.Parse(new ParseRequest<TContext>
                    {
                        Context = request.Context,
                        Formula = resolverResponse.ResolverResults[0].Value,
                        PreviousResponse = resolverResponse
                    });

                    var innerExpressionResult = Expression.Constant(parseResponse.Result);
                    return innerExpressionResult;
                }

                var listValues = resolverResponse.ResolverResults
                    .Select(result => _language.Parse(new ParseRequest<TContext>
                    {
                        Context = request.Context,
                        Formula = result.Value
                    }))
                    .Select(aqlFormula => aqlFormula.Result)
                    .ToList();

                var innerListResult = Expression.Constant(listValues);
                return innerListResult;
            }

            var innerEmptyResult = Expression.Constant(0);
            return innerEmptyResult;
        }

        private ConstantExpression ResolveCell(Stack<Operand> brackets, ParseRequest<TContext> request)
        {
            var idToBeResolved = new List<string>();
            foreach (var bracket in brackets)
            {
                var le = Expression.Lambda<Func<string>>(bracket.Expression);
                var compiledExpression = le.Compile();

                idToBeResolved.Add(compiledExpression());
            }

            var idsToResolve = string.Join(",", idToBeResolved);

            var resolverResponse = _language.Resolver.ReferenceResolver(_language, new ResolverRequest<TContext>
            {
                Context = request.Context,
                ReferenceId = idsToResolve,
                ReferenceType = ReferenceType.TableCell
            }).Result;

            if (resolverResponse.ResolverResults is { } || resolverResponse.ResolverResults.Count > 0)
            {
                if (resolverResponse.ResolverResults.Count == 1 && !resolverResponse.ReferenceAllowsMultipleValues)
                {
                    var parseResponse = _language.Parse(new ParseRequest<TContext>
                    {
                        Context = request.Context,
                        Formula = resolverResponse.ResolverResults[0].Value
                    });

                    var innerExpressionResult = Expression.Constant(parseResponse.Result);

                    return innerExpressionResult;
                }

                var listValues = resolverResponse.ResolverResults
                    .Select(result => _language.Parse(new ParseRequest<TContext>
                    {
                        Context = request.Context,
                        Formula = result.Value
                    }))
                    .Select(aqlFormula => aqlFormula.Result)
                    .ToList();

                var innerListResult = Expression.Constant(listValues);
                return innerListResult;
            }

            var innerEmptyResult = Expression.Constant(0);
            return innerEmptyResult;
        }
    }
}
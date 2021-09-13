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

    public class SquareBracketOpenDefinition : GrammarDefinition
    {
        private readonly AlbedoQueryLanguage _language;

        public SquareBracketOpenDefinition(Grammar grammar, AlbedoQueryLanguage language) : base(grammar)
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

            if (bracketOperands.Count > 2)
            {
                var operandSpan = StringSegment.Encompass(bracketOperands.Skip(1).Select(x => x.StringSegment));
                throw new OperandUnexpectedException(operandSpan);
            }

            if (bracketOperands.Count == 2)
            {
                // table reference
                var (expression, innerDep) = ResolveCell(bracketOperands);
                
                var bracketOperand = bracketOperands.Pop();
                
                var sourceMap = StringSegment.Encompass(
                    bracketOpen.StringSegment,
                    bracketOperand.StringSegment,
                    bracketClose.StringSegment);

                state.Operands.Push(new Operand(expression, sourceMap, innerDep));
            }
            else
            {
                var bracketOperand = bracketOperands.Pop();
                var (expression, innerDep) = Resolve(bracketOperand);

                var sourceMap = StringSegment.Encompass(
                    bracketOpen.StringSegment,
                    bracketOperand.StringSegment,
                    bracketClose.StringSegment);

                state.Operands.Push(new Operand(expression, sourceMap, innerDep));
            }
        }

        private (ConstantExpression, InnerDep) Resolve(Operand bracketOperand)
        {
            var le = Expression.Lambda<Func<string>>(bracketOperand.Expression);
            var compiledExpression = le.Compile();
            var idToBeResolved = compiledExpression();

            var response = _language.Resolver.ReferenceResolver(new ResolverRequest
            {
                InputId = idToBeResolved,
                InputType = InputType.Resolver
            }).Result;

            if (response.Values.Count == 1 && !response.AllowsMultipleValues)
            {
                var aqlFormula = _language.Parse(response.Values[0]);
                var innerExpressionResult = Expression.Constant(aqlFormula.Result);

                return (innerExpressionResult, new InnerDep(aqlFormula, response));
            }

            var listValues = response.Values
                .Select(value => _language.Parse(value))
                .Select(aqlFormula => aqlFormula.Result)
                .ToList();

            var innerListResult = Expression.Constant(listValues);
            return (innerListResult, null);
        }
        
        private (ConstantExpression, InnerDep) ResolveCell(Stack<Operand> brackets)
        {
            var idToBeResolved = new List<string>();
            foreach (var bracket in brackets)
            {
                var le = Expression.Lambda<Func<string>>(bracket.Expression);
                var compiledExpression = le.Compile();
                
                idToBeResolved.Add(compiledExpression());
            }

            var idsToResolve = string.Join(",", idToBeResolved);
            
            var response = _language.Resolver.ReferenceResolver(new ResolverRequest
            {
                InputId = idsToResolve,
                InputType = InputType.Resolver
            }).Result;

            if (response.Values.Count == 1 && !response.AllowsMultipleValues)
            {
                var aqlFormula = _language.Parse(response.Values[0]);
                var innerExpressionResult = Expression.Constant(aqlFormula.Result);

                return (innerExpressionResult, new InnerDep(aqlFormula, response));
            }

            var listValues = response.Values
                .Select(value => _language.Parse(value))
                .Select(aqlFormula => aqlFormula.Result)
                .ToList();

            var innerListResult = Expression.Constant(listValues);
            return (innerListResult, null);
        }
    }
}
namespace AlbedoTeam.Sdk.QueryLanguage.Core.States
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Injections;
    using Structs;

    public class ParsingState<TContext>
        where TContext : IResolverContext
    {
        public ParseRequest<TContext> Request { get; set; } = null;
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();
        public Stack<Operand> Operands { get; } = new Stack<Operand>();
        public Stack<Operator<TContext>> Operators { get; } = new Stack<Operator<TContext>>();
    }
}
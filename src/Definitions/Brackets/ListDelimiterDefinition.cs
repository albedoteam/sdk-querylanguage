namespace AlbedoTeam.Sdk.QueryLanguage.Definitions.Brackets
{
    using Core.States;
    using Core.Structs;
    using Exceptions;
    using Injections;

    public class ListDelimiterDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        public ListDelimiterDefinition(Grammar grammar) : base(grammar)
        {
        }

        public override void Apply(Token<TContext> token, ParsingState<TContext> state)
        {
            state.Operators.Push(new Operator<TContext>(
                this,
                token.StringSegment,
                () => throw new ListDelimeterNotWithinBrackets(token.StringSegment)));
        }
    }
}
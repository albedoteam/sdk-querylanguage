namespace AlbedoTeam.Sdk.QueryLanguage.Definitions
{
    using Core.States;
    using Core.Structs;
    using Injections;

    public class WhitespaceDefinition<TContext> : GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        public WhitespaceDefinition(Grammar grammar) : base(grammar)
        {
        }

        public override void Apply(Token<TContext> token, ParsingState<TContext> state)
        {
        }
    }
}
namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using Definitions;
    using Injections;

    public readonly struct Token<TContext>
        where TContext : IResolverContext
    {
        public Token(
            GrammarDefinition<TContext> definition,
            string value,
            StringSegment stringSegment,
            ParseRequest<TContext> request)
        {
            Definition = definition;
            Value = value;
            StringSegment = stringSegment;
            Request = request;
        }

        public GrammarDefinition<TContext> Definition { get; }
        public StringSegment StringSegment { get; }
        public string Value { get; }
        public ParseRequest<TContext> Request { get; }
    }
}
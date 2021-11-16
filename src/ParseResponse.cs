namespace AlbedoTeam.Sdk.QueryLanguage
{
    using Injections;

    public class ParseResponse<TContext>
        where TContext : IResolverContext
    {
        public ParseRequest<TContext> Request { get; set; }
        public object Result { get; set; }
        public bool NeedsAdicionalParse { get; set; }
        public string AditionalParseFormula { get; set; }
    }
}
namespace AlbedoTeam.Sdk.QueryLanguage
{
    using Injections;

    public class ParseRequest<TContext>
        where TContext : IResolverContext
    {
        public TContext Context { get; set; }
        public string Formula { get; set; }
        public IResolverResponse<TContext> PreviousResponse { get; set; }
    }
}
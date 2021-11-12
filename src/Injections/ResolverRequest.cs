namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    public class ResolverRequest<TContext> : IResolverRequest<TContext>
        where TContext : IResolverContext
    {
        public TContext Context { get; set; }
        public ReferenceType ReferenceType { get; set; }
        public string ReferenceId { get; set; }
        public IResolverResponse<TContext> PreviousResponse { get; set; }
    }
}
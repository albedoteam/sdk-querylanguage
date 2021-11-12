namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    public interface IResolverRequest<TContext>
        where TContext : IResolverContext
    {
        TContext Context { get; set; }
        ReferenceType ReferenceType { get; set; }
        string ReferenceId { get; set; }
        IResolverResponse<TContext> PreviousResponse { get; set; }
    }
}
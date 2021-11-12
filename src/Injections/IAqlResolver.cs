namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    using System.Threading.Tasks;
    using Core.Languages;

    public interface IAqlResolver<TContext>
        where TContext : IResolverContext
    {
        Task<IResolverResponse<TContext>> ReferenceResolver(
            AlbedoQueryLanguage<TContext> language,
            IResolverRequest<TContext> request);
    }
}
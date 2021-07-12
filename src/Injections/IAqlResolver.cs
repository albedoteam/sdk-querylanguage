namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    using System.Threading.Tasks;

    public interface IAqlResolver
    {
        Task<ResolverResponse> ReferenceResolver(ResolverRequest request);
    }
}
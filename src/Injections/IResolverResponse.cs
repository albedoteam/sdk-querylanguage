namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    using System.Collections.Generic;

    public interface IResolverResponse<TContext>
        where TContext : IResolverContext
    {
        IResolverRequest<TContext> Request { get; set; }
        string ReferenceName { get; set; }
        bool ReferenceAllowsMultipleValues { get; set; }
        List<IResolverResult> ResolverResults { get; set; }
        bool WasSolved { get; }
    }
}
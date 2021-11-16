namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    using System.Collections.Generic;

    public class ResolverResponse<TContext> : IResolverResponse<TContext>
        where TContext : IResolverContext
    {
        private ResolverResponse()
        {
        }

        public IResolverRequest<TContext> Request { get; set; }
        public string ReferenceName { get; set; }
        public bool ReferenceAllowsMultipleValues { get; set; }
        public List<IResolverResult> ResolverResults { get; set; }
        public bool WasSolved { get; private set; }

        public static ResolverResponse<TContext> Solved(
            IResolverRequest<TContext> request,
            string referenceName,
            bool referenceAllowsMultipleValues,
            List<IResolverResult> resolverResults)
        {
            return new ResolverResponse<TContext>
            {
                WasSolved = true,
                Request = request,
                ReferenceName = referenceName,
                ReferenceAllowsMultipleValues = referenceAllowsMultipleValues,
                ResolverResults = resolverResults
            };
        }

        public static ResolverResponse<TContext> NotSolved(IResolverRequest<TContext> request)
        {
            return new ResolverResponse<TContext>
            {
                Request = request,
                WasSolved = false
            };
        }
    }
}
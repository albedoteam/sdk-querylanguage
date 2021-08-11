namespace AlbedoTeam.Sdk.QueryLanguage.Core.Languages
{
    using Injections;

    public class InnerDep
    {
        public InnerDep(AqlFormula aqlFormula, ResolverResponse response)
        {
            Formula = aqlFormula;
            Resolver = response;
        }

        public AqlFormula Formula { get; set; }
        public ResolverResponse Resolver { get; set; }
    }
}
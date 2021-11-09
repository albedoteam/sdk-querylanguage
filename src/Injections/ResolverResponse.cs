namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    using System.Collections.Generic;

    public class ResolverResponse
    {
        public InputType Type { get; set; }
        public string Id { get; set; }
        public string Nome { get; set; }
        public bool AllowsMultipleValues { get; set; }
        public List<ResolverResult> ResolverResults { get; set; }

        public ResolverResponse()
        {
            ResolverResults = new List<ResolverResult>();
        }
        
        public class ResolverResult
        {
            public Dictionary<string, string> Metadata { get; set; }
            public string Value { get; set; }

            public ResolverResult()
            {
                Metadata = new Dictionary<string, string>();
            }
        }
    }
}
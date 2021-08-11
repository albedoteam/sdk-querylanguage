namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    using System.Collections.Generic;

    public class ResolverResponse
    {
        public InputType Type { get; set; }
        public string Id { get; set; }
        public string Nome { get; set; }
        public bool AllowsMultipleValues { get; set; }
        public List<string> Values { get; set; }
    }
}
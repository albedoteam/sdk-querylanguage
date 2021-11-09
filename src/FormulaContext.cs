namespace AlbedoTeam.Sdk.QueryLanguage
{
    using System.Collections.Generic;

    public class FormulaContext
    {
        public Dictionary<string, string> Metadata { get; set; }
        public string Formula { get; set; }
    }
}
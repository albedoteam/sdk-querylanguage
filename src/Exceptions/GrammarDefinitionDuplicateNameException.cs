namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using System;

    public class GrammarDefinitionDuplicateNameException : Exception
    {
        public readonly string GrammarDefinitionName;

        public GrammarDefinitionDuplicateNameException(string grammarDefinitionName) : base(
            $"Grammer definition name '{grammarDefinitionName}' has been defined multiple times")
        {
            GrammarDefinitionName = grammarDefinitionName;
        }
    }
}
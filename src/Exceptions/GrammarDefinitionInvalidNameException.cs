namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using System;

    public class GrammarDefinitionInvalidNameException : Exception
    {
        public GrammarDefinitionInvalidNameException(string name) :
            base($"Invalid grammer definition name '{name}' name may only contain [a-zA-Z0-9_]")
        {
        }
    }
}
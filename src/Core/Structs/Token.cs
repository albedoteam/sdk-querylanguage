namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using Definitions;

    public readonly struct Token
    {
        public Token(GrammarDefinition definition, string value, StringSegment stringSegment)
        {
            Definition = definition;
            Value = value;
            StringSegment = stringSegment;
        }

        public GrammarDefinition Definition { get; }
        public StringSegment StringSegment { get; }
        public string Value { get; }
    }
}
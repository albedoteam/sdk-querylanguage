namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using Definitions;

    public readonly struct Token
    {
        public Token(
            GrammarDefinition definition,
            string value,
            StringSegment stringSegment,
            FormulaContext context)
        {
            Definition = definition;
            Value = value;
            StringSegment = stringSegment;
            Context = context;
        }

        public GrammarDefinition Definition { get; }
        public StringSegment StringSegment { get; }
        public string Value { get; }
        public FormulaContext Context { get; }
    }
}
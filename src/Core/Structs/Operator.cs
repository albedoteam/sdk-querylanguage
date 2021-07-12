namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using System;
    using Definitions;

    public struct Operator
    {
        public Operator(GrammarDefinition definition, StringSegment stringSegment, Action execute)
        {
            Definition = definition;
            Execute = execute;
            StringSegment = stringSegment;
        }

        public GrammarDefinition Definition { get; }
        public Action Execute { get; }
        public StringSegment StringSegment { get; }

        public override string ToString()
        {
            return StringSegment.ToString();
        }
    }
}
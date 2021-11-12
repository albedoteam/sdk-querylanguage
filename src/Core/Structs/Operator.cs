namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using System;
    using Definitions;
    using Injections;

    public struct Operator<TContext>
        where TContext : IResolverContext
    {
        public Operator(GrammarDefinition<TContext> definition, StringSegment stringSegment, Action execute)
        {
            Definition = definition;
            Execute = execute;
            StringSegment = stringSegment;
        }

        public GrammarDefinition<TContext> Definition { get; }
        public Action Execute { get; }
        public StringSegment StringSegment { get; }

        public override string ToString()
        {
            return StringSegment.ToString();
        }
    }
}
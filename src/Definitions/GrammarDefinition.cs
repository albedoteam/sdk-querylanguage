namespace AlbedoTeam.Sdk.QueryLanguage.Definitions
{
    using System.Text.RegularExpressions;
    using Core.States;
    using Core.Structs;
    using Exceptions;
    using Injections;

    public abstract class GrammarDefinition<TContext>
        where TContext : IResolverContext
    {
        private static readonly Regex NameValidation = new Regex("^[a-zA-Z0-9_]+$");

        protected GrammarDefinition(Grammar grammar)
        {
            if (!NameValidation.IsMatch(grammar.Name))
                throw new GrammarDefinitionInvalidNameException(grammar.Name);

            Grammar = grammar;
        }

        public Grammar Grammar { get; }

        public abstract void Apply(Token<TContext> token, ParsingState<TContext> state);
    }
}
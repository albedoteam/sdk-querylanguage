namespace AlbedoTeam.Sdk.QueryLanguage.Definitions
{
    using Core.States;
    using Core.Structs;

    public class WhitespaceDefinition : GrammarDefinition
    {
        public WhitespaceDefinition(Grammar grammar) : base(grammar)
        {
        }

        public override void Apply(Token token, ParsingState state)
        {
        }
    }
}
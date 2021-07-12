namespace AlbedoTeam.Sdk.QueryLanguage.Definitions.Brackets
{
    using Core.States;
    using Core.Structs;
    using Exceptions;

    public class ListDelimiterDefinition : GrammarDefinition
    {
        public ListDelimiterDefinition(Grammar grammar) : base(grammar)
        {
        }

        public override void Apply(Token token, ParsingState state)
        {
            state.Operators.Push(new Operator(
                this,
                token.StringSegment,
                () => throw new ListDelimeterNotWithinBrackets(token.StringSegment)));
        }
    }
}
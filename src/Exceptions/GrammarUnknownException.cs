namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using Core.Structs;

    internal class GrammarUnknownException : ParseException
    {
        public readonly StringSegment UnexpectedGrammarStringSegment;

        public GrammarUnknownException(StringSegment unexpectedGrammarStringSegment)
            : base(unexpectedGrammarStringSegment, "Unexpected token " +
                                                   $"'{unexpectedGrammarStringSegment.ToString()}' found")
        {
            UnexpectedGrammarStringSegment = unexpectedGrammarStringSegment;
        }
    }
}
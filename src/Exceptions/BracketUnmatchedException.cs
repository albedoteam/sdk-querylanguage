namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using Core.Structs;

    internal class BracketUnmatchedException : ParseException
    {
        public readonly StringSegment BracketStringSegment;

        public BracketUnmatchedException(StringSegment bracketStringSegment)
            : base(bracketStringSegment, $"Bracket '{bracketStringSegment.ToString()}' is unmatched")
        {
            BracketStringSegment = bracketStringSegment;
        }
    }
}
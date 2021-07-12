namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using Core.Structs;

    internal class ListDelimeterNotWithinBrackets : ParseException
    {
        public readonly StringSegment DelimeterStringSegment;

        public ListDelimeterNotWithinBrackets(StringSegment delimeterStringSegment)
            : base(delimeterStringSegment,
                $"List delimeter '{delimeterStringSegment.ToString()}' is not within brackets")
        {
            DelimeterStringSegment = delimeterStringSegment;
        }
    }
}
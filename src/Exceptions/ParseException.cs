namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using System;
    using Core.Structs;

    internal abstract class ParseException : Exception
    {
        public readonly StringSegment ErrorSegment;

        public ParseException(StringSegment errorSegment, string message)
            : base(message)
        {
            ErrorSegment = errorSegment;
        }

        public ParseException(StringSegment errorSegment, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorSegment = errorSegment;
        }
    }
}
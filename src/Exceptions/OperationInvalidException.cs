namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using System;
    using Core.Structs;

    internal class OperationInvalidException : ParseException
    {
        public OperationInvalidException(StringSegment errorSegment, Exception innerException)
            : base(errorSegment, $"Unable to perform operation '{errorSegment}'", innerException)
        {
        }
    }
}
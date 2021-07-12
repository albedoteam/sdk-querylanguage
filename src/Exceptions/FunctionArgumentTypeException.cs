namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using System;
    using Core.Structs;

    internal class FunctionArgumentTypeException : ParseException
    {
        public readonly Type ActualType;
        public readonly StringSegment ArgumentStringSegment;
        public readonly Type ExpectedType;

        public FunctionArgumentTypeException(
            StringSegment argumentStringSegment,
            Type expectedType,
            Type actualType)
            : base(
                argumentStringSegment,
                $"Argument '{argumentStringSegment.ToString()}' type expected {expectedType} but was {actualType}")
        {
            ArgumentStringSegment = argumentStringSegment;
            ExpectedType = expectedType;
            ActualType = actualType;
        }
    }
}
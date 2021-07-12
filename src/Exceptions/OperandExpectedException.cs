namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using Core.Structs;

    internal class OperandExpectedException : ParseException
    {
        public readonly StringSegment ExpectedOperandStringSegment;
        public readonly StringSegment OperatorStringSegment;

        public OperandExpectedException(StringSegment expectedOperandStringSegment)
            : base(expectedOperandStringSegment, "Expected operands to be found")
        {
            ExpectedOperandStringSegment = expectedOperandStringSegment;
        }

        public OperandExpectedException(StringSegment operatorStringSegment, StringSegment expectedOperandStringSegment)
            : base(expectedOperandStringSegment, "Expected operands to be found for " +
                                                 $"'{operatorStringSegment.ToString()}'")
        {
            OperatorStringSegment = operatorStringSegment;
            ExpectedOperandStringSegment = expectedOperandStringSegment;
        }
    }
}
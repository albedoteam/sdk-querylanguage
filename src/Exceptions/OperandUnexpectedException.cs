namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using Core.Structs;

    internal class OperandUnexpectedException : ParseException
    {
        public readonly StringSegment OperatorStringSegment;
        public readonly StringSegment UnexpectedOperandStringSegment;

        public OperandUnexpectedException(StringSegment unexpectedOperandStringSegment)
            : base(unexpectedOperandStringSegment,
                $"Unexpected operands '{unexpectedOperandStringSegment.ToString()}' found. " +
                "Perhaps an operator is missing")
        {
            UnexpectedOperandStringSegment = unexpectedOperandStringSegment;
        }

        public OperandUnexpectedException(StringSegment operatorStringSegment,
            StringSegment unexpectedOperandStringSegment)
            : base(unexpectedOperandStringSegment,
                $"Unexpected operands '{unexpectedOperandStringSegment.ToString()}' found while processing " +
                $"'{operatorStringSegment.ToString()}'. Perhaps an operator is missing")
        {
            OperatorStringSegment = operatorStringSegment;
            UnexpectedOperandStringSegment = unexpectedOperandStringSegment;
        }
    }
}
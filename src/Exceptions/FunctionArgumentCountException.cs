namespace AlbedoTeam.Sdk.QueryLanguage.Exceptions
{
    using Core.Structs;

    internal class FunctionArgumentCountException : ParseException
    {
        public readonly int ActualOperandCount;
        public readonly StringSegment BracketStringSegment;
        public readonly int ExpectedOperandCount;

        public FunctionArgumentCountException(
            StringSegment bracketStringSegment,
            int expectedOperandCount,
            int actualOperandCount)
            : base(
                bracketStringSegment,
                $"Bracket '{bracketStringSegment.ToString()}' contains {actualOperandCount} " +
                $"operand{(actualOperandCount > 1 ? "s" : "")} but was expecting {expectedOperandCount} " +
                $"operand{(expectedOperandCount > 1 ? "s" : "")}")
        {
            BracketStringSegment = bracketStringSegment;
            ExpectedOperandCount = expectedOperandCount;
            ActualOperandCount = actualOperandCount;
        }
    }
}
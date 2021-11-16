namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using System.Linq.Expressions;

    public readonly struct Operand
    {
        public Operand(
            Expression expression,
            StringSegment stringSegment)
        {
            Expression = expression;
            StringSegment = stringSegment;
        }

        public Expression Expression { get; }
        public StringSegment StringSegment { get; }

        public override string ToString()
        {
            return StringSegment.ToString();
        }
    }
}
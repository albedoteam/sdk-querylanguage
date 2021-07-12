namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using System.Linq.Expressions;
    using Languages;

    public readonly struct Operand
    {
        public Operand(
            Expression expression,
            StringSegment stringSegment,
            InnerDep innerFormula = null)
        {
            Expression = expression;
            StringSegment = stringSegment;
            InnerDep = innerFormula;
        }

        public Expression Expression { get; }
        public StringSegment StringSegment { get; }
        public InnerDep InnerDep { get; }

        public override string ToString()
        {
            return StringSegment.ToString();
        }
    }
}
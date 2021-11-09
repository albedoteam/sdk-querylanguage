namespace AlbedoTeam.Sdk.QueryLanguage.Core.States
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Structs;

    public class ParsingState
    {
        public FormulaContext Context { get; set; } = null;
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();
        public Stack<Operand> Operands { get; } = new Stack<Operand>();
        public Stack<Operator> Operators { get; } = new Stack<Operator>();
    }
}
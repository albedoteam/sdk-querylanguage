namespace AlbedoTeam.Sdk.QueryLanguage.Core.Languages
{
    using System.Collections.Generic;
    using Structs;

    public class AqlFormula
    {
        public AqlFormula()
        {
            InnerOps = new List<Operand>();
        }

        public string ExpressionText { get; set; }
        public string Raw { get; set; }
        public object Result { get; set; }

        public List<Operand> InnerOps { get; set; }
    }
}
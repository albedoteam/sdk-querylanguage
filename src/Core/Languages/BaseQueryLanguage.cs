namespace AlbedoTeam.Sdk.QueryLanguage.Core.Languages
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Definitions;
    using Structs;

    internal class BaseQueryLanguage
    {
        protected readonly Parser Parser;
        protected readonly Tokenizer Tokenizer;

        public BaseQueryLanguage(params GrammarDefinition[] grammarDefintions)
        {
            Tokenizer = new Tokenizer(grammarDefintions);
            Parser = new Parser();
        }

        public IReadOnlyList<GrammarDefinition> TokenDefinitions => Tokenizer.GrammarDefinitions;

        // public (Expression, Dictionary<string, Operand>) Parse(string text, params ParameterExpression[] parameters)
        // {
        //     var tokenStream = Tokenizer.Tokenize(text);
        //     return Parser.Parse(tokenStream, parameters);
        // }
        
        public (Expression, Dictionary<string, Operand>) Parse(FormulaContext context, params ParameterExpression[] parameters)
        {
            var tokenStream = Tokenizer.Tokenize(context);
            return Parser.Parse(tokenStream, parameters);
        }
    }
}
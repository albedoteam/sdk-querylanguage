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

        public (Expression, Dictionary<string, Operand>) Parse(string text, params ParameterExpression[] parameters)
        {
            var tokenStream = Tokenizer.Tokenize(text);
            return Parser.Parse(tokenStream, parameters);
        }

        // public Expression<Func<TOut>> Parse<TOut>(string text)
        // {
        //     var body = Parse(text);
        //     return Expression.Lambda<Func<TOut>>(body);
        // }
        //
        // public Expression<Func<TIn, TOut>> Parse<TIn, TOut>(string text)
        // {
        //     var parameters = new[] {Expression.Parameter(typeof(TIn))};
        //     var body = Parse(text, parameters);
        //     return Expression.Lambda<Func<TIn, TOut>>(body, parameters);
        // }
    }
}
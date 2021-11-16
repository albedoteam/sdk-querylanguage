namespace AlbedoTeam.Sdk.QueryLanguage.Core.Languages
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Definitions;
    using Injections;

    internal class BaseQueryLanguage<TContext>
        where TContext : IResolverContext
    {
        protected readonly Parser<TContext> Parser;
        protected readonly Tokenizer<TContext> Tokenizer;

        public BaseQueryLanguage(params GrammarDefinition<TContext>[] grammarDefintions)
        {
            Tokenizer = new Tokenizer<TContext>(grammarDefintions);
            Parser = new Parser<TContext>();
        }

        public IReadOnlyList<GrammarDefinition<TContext>> TokenDefinitions => Tokenizer.GrammarDefinitions;

        // public (Expression, Dictionary<string, Operand>) Parse(string text, params ParameterExpression[] parameters)
        // {
        //     var tokenStream = Tokenizer.Tokenize(text);
        //     return Parser.Parse(tokenStream, parameters);
        // }

        public Expression Parse(ParseRequest<TContext> request, params ParameterExpression[] parameters)
        {
            var tokenStream = Tokenizer.Tokenize(request);
            return Parser.Parse(tokenStream, parameters);
        }
    }
}
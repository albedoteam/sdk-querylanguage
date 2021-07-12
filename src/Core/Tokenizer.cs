namespace AlbedoTeam.Sdk.QueryLanguage.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Definitions;
    using Exceptions;
    using Structs;

    internal class Tokenizer
    {
        public readonly IReadOnlyList<GrammarDefinition> GrammarDefinitions;
        protected readonly Regex TokenRegex;

        public Tokenizer(params GrammarDefinition[] grammarDefinitions)
        {
            var duplicateKey = grammarDefinitions.GroupBy(g => g.Grammar.Name).FirstOrDefault(g => g.Count() > 1)?.Key;
            if (duplicateKey != null)
                throw new GrammarDefinitionDuplicateNameException(duplicateKey);

            GrammarDefinitions = grammarDefinitions.ToList();

            var pattern = string.Join("|", GrammarDefinitions.Select(g => $"(?<{g.Grammar.Name}>{g.Grammar.Regex})"));
            TokenRegex = new Regex(pattern);
        }

        public IEnumerable<Token> Tokenize(string text)
        {
            var matches = TokenRegex.Matches(text).OfType<Match>();

            var expectedIndex = 0;
            foreach (var match in matches)
            {
                if (match.Index > expectedIndex)
                    throw new GrammarUnknownException(new StringSegment(text, expectedIndex,
                        match.Index - expectedIndex));

                expectedIndex = match.Index + match.Length;

                var matchedTokenDefinition =
                    GrammarDefinitions.FirstOrDefault(g => match.Groups[g.Grammar.Name].Success);
                if (matchedTokenDefinition is {Grammar: {Ignore: true}})
                    continue;

                yield return new Token(
                    matchedTokenDefinition,
                    match.Value,
                    new StringSegment(text, match.Index, match.Length));
            }
        }
    }
}
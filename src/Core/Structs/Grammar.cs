namespace AlbedoTeam.Sdk.QueryLanguage.Core.Structs
{
    using System;

    public readonly struct Grammar
    {
        public Grammar(string name, string regex, bool ignore = false)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
            Ignore = ignore;
        }

        public string Name { get; }
        public string Regex { get; }
        public bool Ignore { get; }
    }
}
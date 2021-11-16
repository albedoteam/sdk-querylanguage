namespace AlbedoTeam.Sdk.QueryLanguage.Injections
{
    using System;

    public interface IResolverContext
    {
        Guid Id { get; set; }
        DateTime CreationDate { get; set; }
    }
}
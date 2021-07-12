namespace AlbedoTeam.Sdk.QueryLanguage.Core.Utility
{
    using System;
    using System.Reflection;

    internal static class TypeShim
    {
        public static PropertyInfo GetProperty(Type type, string property)
        {
            return type.GetTypeInfo().GetProperty(property,
                BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
        }
    }
}
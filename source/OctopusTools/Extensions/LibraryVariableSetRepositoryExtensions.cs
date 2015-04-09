using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace OctopusTools.Extensions
{
    public static class LibraryVariableSetRepositoryExtensions
    {
        public static LibraryVariableSetResource FindByName(this ILibraryVariableSetRepository repo, string name)
        {
            name = (name ?? string.Empty).Trim();
            return repo.FindOne(r => string.Equals((r.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

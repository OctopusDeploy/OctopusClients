using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Operations;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Util;

internal static class AsyncRepositoryExtensions
{
    public static async Task<List<TResource>> FindByNameIdOrSlugs<TResource, TRepository>(this TRepository repository,
        string[] searchIdentifiers, Func<string[], string> exceptionMessageGenerator = null)
        where TRepository : IFindBySlug<TResource>, IFindByName<TResource>, IGet<TResource>
        where TResource : IResource, INamedResource, IHaveSlugResource
    {
        List<TResource> resources = new();

        var resourcesByName = await repository.FindByNames(searchIdentifiers).ConfigureAwait(false);
        resources.AddRange(resourcesByName);

        var missing = searchIdentifiers
            .Except(resourcesByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase)
            .ToArray();

        // use the missing names to try and find by slug
        var resourcesBySlug = await repository.FindBySlugs(missing, CancellationToken.None)
            .ConfigureAwait(false);
        resources.AddRange(resourcesBySlug);

        missing = missing
            .Except(resourcesBySlug.Select(e => e.Slug), StringComparer.OrdinalIgnoreCase)
            .ToArray();

        // any other missing slugs/names could be Id's, so look again
        var resourcesByIds = await repository.Get(missing).ConfigureAwait(false);
        resources.AddRange(resourcesByIds);

        missing = missing
            .Except(resourcesByIds.Select(e => e.Id), StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (missing.Any())
            throw new InvalidRegistrationArgumentsException(exceptionMessageGenerator is not null
                ? exceptionMessageGenerator(missing)
                : $"Failed to find the following: {string.Join(", ", missing)}");

        return resources;
    }
}
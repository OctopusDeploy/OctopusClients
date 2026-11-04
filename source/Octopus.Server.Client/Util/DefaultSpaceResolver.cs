using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Client.Util
{
    /// <summary>
    /// Resolves the current user's default space, for use when a repository is not explicitly scoped to a space.
    /// </summary>
    internal static class DefaultSpaceResolver
    {
        /// <summary>
        /// Returns the current user's default space, or null if the server doesn't support spaces,
        /// the user isn't permitted to list their spaces, or none of their spaces is the default.
        /// </summary>
        public static SpaceResource TryGetDefaultSpace(IOctopusClient client, RootResource rootDocument)
        {
            if (!rootDocument.HasLink("Spaces"))
                return null;

            try
            {
                var currentUser = client.Get<UserResource>(rootDocument.Links["CurrentUser"]);
                var userSpaces = client.Get<SpaceResource[]>(currentUser.Links["Spaces"]);
                return userSpaces.SingleOrDefault(s => s.IsDefault);
            }
            catch (OctopusSecurityException)
            {
                return null;
            }
        }

        /// <inheritdoc cref="TryGetDefaultSpace"/>
        public static async Task<SpaceResource> TryGetDefaultSpaceAsync(IOctopusAsyncClient client, RootResource rootDocument, CancellationToken cancellationToken)
        {
            if (!rootDocument.HasLink("Spaces"))
                return null;

            try
            {
                var currentUser = await client.Get<UserResource>(rootDocument.Links["CurrentUser"], cancellationToken).ConfigureAwait(false);
                var userSpaces = await client.Get<SpaceResource[]>(currentUser.Links["Spaces"], cancellationToken).ConfigureAwait(false);
                return userSpaces.SingleOrDefault(s => s.IsDefault);
            }
            catch (OctopusSecurityException)
            {
                return null;
            }
        }
    }
}

using System;
using System.Threading.Tasks;

namespace Octopus.Client
{
    public class RepositoryScope
    {
        public static RepositoryScope ForSpace(string spaceId) => new RepositoryScope(RepositoryScopeType.Space, spaceId);
        public static RepositoryScope ForSystem() => new RepositoryScope(RepositoryScopeType.System, null);
        public static RepositoryScope Unspecified() => new RepositoryScope(RepositoryScopeType.Unspecified, null);

        private readonly RepositoryScopeType type;
        private readonly string spaceId;

        private RepositoryScope(RepositoryScopeType type, string spaceId)
        {
            if (type == RepositoryScopeType.Space && string.IsNullOrEmpty(spaceId))
            {
                throw new Exception("invalid");
            }

            this.type = type;
            this.spaceId = spaceId;
        }

        private enum RepositoryScopeType
        {
            Space,
            System,
            Unspecified
        }

        public T Apply<T>(Func<string, T> whenSpaceScoped, Func<T> whenSystemScoped, Func<T> whenUnspecifiedScope)
        {
            switch (type)
            {
                case RepositoryScopeType.Space:
                    return whenSpaceScoped(spaceId);
                case RepositoryScopeType.System:
                    return whenSystemScoped();
                case RepositoryScopeType.Unspecified:
                    return whenUnspecifiedScope();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Task<SpaceContext> ToSpaceContext(IOctopusSpaceAsyncRepository repo)
        {
            return Apply(id => Task.FromResult(SpaceContext.SpecificSpace(id)),
                () => Task.FromResult(SpaceContext.SystemOnly()),
                async () =>
                {
                    var spaceRootDocument = await repo.LoadSpaceRootDocument().ConfigureAwait(false);
                    return spaceRootDocument == null ? SpaceContext.SystemOnly() : SpaceContext.DefaultSpaceAndSystem();
                });
        }
    }
}
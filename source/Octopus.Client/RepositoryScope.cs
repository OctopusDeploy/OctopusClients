using System;
using Octopus.Client.Model;

namespace Octopus.Client
{
    public class RepositoryScope
    {
        public static RepositoryScope ForSpace(SpaceResource space) => new RepositoryScope(RepositoryScopeType.Space, space);
        public static RepositoryScope ForSystem() => new RepositoryScope(RepositoryScopeType.System, null);
        public static RepositoryScope Unspecified() => new RepositoryScope(RepositoryScopeType.Unspecified, null);

        private readonly RepositoryScopeType type;
        private readonly SpaceResource space;

        private RepositoryScope(RepositoryScopeType type, SpaceResource space)
        {
            if (type == RepositoryScopeType.Space && space == null)
            {
                throw new ArgumentNullException(nameof(space));
            }

            this.type = type;
            this.space = space;
        }

        private enum RepositoryScopeType
        {
            Space,
            System,
            Unspecified
        }

        public T Apply<T>(Func<SpaceResource, T> whenSpaceScoped, Func<T> whenSystemScoped, Func<T> whenUnspecifiedScope)
        {
            switch (type)
            {
                case RepositoryScopeType.Space:
                    return whenSpaceScoped(space);
                case RepositoryScopeType.System:
                    return whenSystemScoped();
                case RepositoryScopeType.Unspecified:
                    return whenUnspecifiedScope();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Apply(Action<SpaceResource> whenSpaceScoped, Action whenSystemScoped, Action whenUnspecifiedScope)
        {
            Apply(space => { whenSpaceScoped(space); return 1; },
                () => { whenSystemScoped(); return 1; },
                () => { whenUnspecifiedScope(); return 1; });
        }
    }
}
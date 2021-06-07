using System;

namespace Octopus.Client.Model
{
    internal static class ProjectResourceExtensionMethods
    {
        internal static void EnsureVersionControlled(this ProjectResource projectResource)
        {
            if (projectResource.PersistenceSettings.Type != PersistenceSettingsType.VersionControlled)
            {
                throw new NotSupportedException("Project must be version controlled.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ReleaseResource : ReleaseSummaryResource, IHaveSpaceResource
    {
        [JsonConstructor]
        public ReleaseResource()
        {
            SelectedPackages = new List<SelectedPackage>();
        }

        public ReleaseResource(string version, string projectId, string channelId) : this()
        {
            Version = version;
            ProjectId = projectId;
            ChannelId = channelId;
        }

        public ReleaseResource(string version, string projectId)
            : this(version, projectId, null)
        {
        }

        public DateTimeOffset Assembled { get; set; }

        [Writeable]
        public string ReleaseNotes { get; set; }

        [WriteableOnCreate]
        public string ProjectId { get; set; }

        [Writeable]
        public string ChannelId { get; set; }

        public string ProjectVariableSetSnapshotId { get; set; }

        /// <summary>
        /// Snapshots of the project's included library variable sets. The
        /// snapshots are <see cref="VariableSetResource" />s, not <see cref="LibraryVariableSetResource" />s.
        /// </summary>
        public List<string> LibraryVariableSetSnapshotIds { get; set; }

        public string ProjectDeploymentProcessSnapshotId { get; set; }
        public List<SelectedPackage> SelectedPackages { get; set; }
        public string SpaceId { get; set; }
    }
}
using System;

namespace Octopus.Client.Model
{
    public class ReleaseTemplatePackage
    {
        public string StepName { get; set; }
    
        public string PackageId { get; set; }
     
        public string FeedId { get; set; }
        public string FeedName { get; set; }

        public string VersionSelectedLastRelease { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PackageId" /> or <see cref="FeedId" /> contain no
        /// references to other variables. Variables can be used to
        /// select different NuGet feeds or packages at deployment time, however, this means that it's not possible to resolve
        /// which feed/package to search when creating a release.
        /// </summary>
        public bool IsResolvable { get; set; }
    }
}
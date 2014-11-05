using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace OctopusTools.Commands
{
    public class ProjectExport
    {
        public ProjectResource Project { get; set; }
        public ReferenceDataItem ProjectGroup { get; set; }
        public VariableSetResource VariableSet { get; set; }
        public List<ReferenceDataItem> NuGetFeeds { get; set; }
        public List<ReferenceDataItem> ActionTemplates { get; set; }
        public DeploymentProcessResource DeploymentProcess { get; set; }
        public List<ReferenceDataItem> LibraryVariableSets { get; set; }
    }
}
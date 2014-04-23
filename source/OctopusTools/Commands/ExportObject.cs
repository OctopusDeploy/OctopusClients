using Octopus.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Commands
{
    public class ExportObject
    {
        public ProjectResource Project { get; set; }
        public ReferenceDataItem ProjectGroup { get; set; }
        public VariableSetResource VariableSet { get; set; }
        public List<ReferenceDataItem> NuGetFeeds { get; set; }
        public DeploymentProcessResource DeploymentProcess { get; set; }
        public List<ReferenceDataItem> LibraryVariableSets { get; set; }
    }
}

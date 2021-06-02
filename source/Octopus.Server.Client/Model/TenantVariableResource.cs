using System.Collections.Generic;
using Octopus.Client.Extensibility;


namespace Octopus.Client.Model
{
    public class TenantVariableResource : Resource, IHaveSpaceResource
    {
        public string TenantId { get; set; }
        public string TenantName { get; set; }

        public Dictionary<string, Project> ProjectVariables { get; set; } = new Dictionary<string, Project>();

        public Dictionary<string, Library> LibraryVariables { get; set; } = new Dictionary<string, Library>();

        public class Project
        {
            public Project(string projectId)
            {
                ProjectId = projectId;
            }

            public string ProjectId { get; }
            public string ProjectName { get; set; }

            public List<ActionTemplateParameterResource> Templates { get; set; } = new List<ActionTemplateParameterResource>();

            public Dictionary<string, Dictionary<string, PropertyValueResource>> Variables { get; set; } = new Dictionary<string, Dictionary<string, PropertyValueResource>>();

            public LinkCollection Links { get; set; }
        }

        public class Library
        {
            public Library(string libraryVariableSetId)
            {
                LibraryVariableSetId = libraryVariableSetId;
            }

            public string LibraryVariableSetId { get;  }
            public string LibraryVariableSetName { get; set; }

            public List<ActionTemplateParameterResource> Templates { get; set; } = new List<ActionTemplateParameterResource>();

            public Dictionary<string, PropertyValueResource> Variables { get; set; } = new Dictionary<string, PropertyValueResource>();

            public LinkCollection Links { get; set; }
        }

        public string SpaceId { get; set; }
    }
}
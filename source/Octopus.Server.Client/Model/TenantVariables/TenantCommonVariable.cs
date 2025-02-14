namespace Octopus.Client.Model.TenantVariables;

public class TenantCommonVariable
{
    public class TenantCommonVariablePayload(string libraryVariableSetId, string templateId, PropertyValueResource value, CommonVariableScope scope)
    {
        public string Id { get; set; } = string.Empty;

        public string LibraryVariableSetId { get; set; } = libraryVariableSetId;
    
        public string LibraryVariableSetName { get; set; }

        public string TemplateId { get; set; } = templateId;

        public ActionTemplateParameterResource Template { get; set; }

        public PropertyValueResource Value { get; set; } = value;

        public CommonVariableScope Scope { get; set; } = scope;
    }
    
    public class CommonVariableScope(ReferenceCollection environmentIds)
    {
        public ReferenceCollection EnvironmentIds { get; set; } = environmentIds;
    }
}
namespace Octopus.Client.Model.TenantVariables;

public class TenantProjectVariable(string projectId, string templateId, PropertyValueResource value, ProjectVariableScope scope)
{
    public string Id { get; set; } = string.Empty;

    public string ProjectId { get; set; } = projectId;

    public string ProjectName { get; set; }

    public string TemplateId { get; set; } = templateId;

    public ActionTemplateParameterResource Template { get; set; }

    public PropertyValueResource Value { get; set; } = value;

    public ProjectVariableScope Scope { get; set; } = scope;
}

public class ProjectVariableScope(ReferenceCollection environmentIds)
{
    public ReferenceCollection EnvironmentIds { get; set; } = environmentIds;
}
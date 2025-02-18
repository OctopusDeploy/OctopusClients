namespace Octopus.Client.Model.TenantVariables;

public class ModifyProjectVariablesByTenantIdCommand(string tenantId, string spaceId, TenantProjectVariablePayload[] variables)
{
    public string TenantId { get; set; } = tenantId;

    public string SpaceId { get; set; } = spaceId;

    public TenantProjectVariablePayload[] Variables { get; set; } = variables;
}

public class TenantProjectVariablePayload(
    string projectId,
    string templateId,
    PropertyValueResource value,
    ProjectVariableScope scope)
{
    public string Id { get; set; } = string.Empty;

    public string ProjectId { get; set; } = projectId;

    public string TemplateId { get; set; } = templateId;

    public PropertyValueResource Value { get; set; } = value;

    public ProjectVariableScope Scope { get; set; } = scope;
}

public class ModifyProjectVariablesByTenantIdResponse(string tenantId, TenantProjectVariable[] projectVariables)
{
    public string TenantId { get; set; } = tenantId;

    public TenantProjectVariable[] ProjectVariables { get; set; } = projectVariables;
}


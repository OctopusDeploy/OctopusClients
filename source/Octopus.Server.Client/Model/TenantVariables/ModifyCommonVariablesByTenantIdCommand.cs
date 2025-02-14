namespace Octopus.Client.Model.TenantVariables;

public class ModifyCommonVariablesByTenantIdCommand(string tenantId, string spaceId, TenantCommonVariablePayload[] variables)
{
    public string TenantId { get; set; } = tenantId;

    public string SpaceId { get; set; } = spaceId;

    public TenantCommonVariablePayload[] Variables { get; set; } = variables;
}


public class TenantCommonVariablePayload(string libraryVariableSetId, string templateId, PropertyValueResource value, TenantCommonVariable.CommonVariableScope scope)
{
    public string Id { get; set; } = string.Empty;

    public string LibraryVariableSetId { get; set; } = libraryVariableSetId;

    public string TemplateId { get; set; } = templateId;

    public PropertyValueResource Value { get; set; } = value;

    public TenantCommonVariable.CommonVariableScope Scope { get; set; } = scope;
}

public class ModifyCommonVariablesByTenantIdResponse(string tenantId, TenantCommonVariable[] commonVariables)
{
    public string TenantId { get; set; } = tenantId;
        
    public TenantCommonVariable[] CommonVariables { get; set; } = commonVariables;
}

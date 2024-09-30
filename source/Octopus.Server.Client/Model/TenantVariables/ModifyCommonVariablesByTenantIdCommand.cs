namespace Octopus.Client.Model.TenantVariables;

public class ModifyCommonVariablesByTenantIdCommand(string tenantId, string spaceId, TenantCommonVariable[] variables)
{
    public string TenantId { get; set; } = tenantId;

    public string SpaceId { get; set; } = spaceId;

    public TenantCommonVariable[] Variables { get; set; } = variables;
}


public class TenantCommonVariable(string libraryVariableSetId, string templateId, PropertyValueResource value, CommonVariableScope scope)
{
    public TenantCommonVariable(string id, string libraryVariableSetId, string templateId, PropertyValueResource value, CommonVariableScope scope) : this(libraryVariableSetId, templateId, value, scope)
    {
        Id = id;
    }

    public string Id { get; set; } = string.Empty;

    public string LibraryVariableSetId { get; set; } = libraryVariableSetId;

    public string TemplateId { get; set; } = templateId;

    public PropertyValueResource Value { get; set; } = value;

    public CommonVariableScope Scope { get; set; } = scope;
}

public class CommonVariableScope(string[] environmentIds)
{
    public string[] EnvironmentIds { get; set; } = environmentIds;
}

public class ModifyCommonVariablesByTenantIdResponse(string tenantId, TenantCommonVariable[] commonVariables)
{
    public string TenantId { get; set; } = tenantId;
        
    public TenantCommonVariable[] CommonVariables { get; set; } = commonVariables;
}
namespace Octopus.Client.Model.TenantVariables;

public class GetProjectVariablesByTenantIdRequest(string tenantId, string spaceId)
{
    public string TenantId { get; set; } = tenantId;

    public string SpaceId { get; set; } = spaceId;
    
    public bool IncludeMissingVariables { get; set; } = false;
}

public class GetProjectVariablesByTenantIdResponse(string tenantId, TenantProjectVariable[] variables)
{
    public string TenantId { get; set; } = tenantId;

    public TenantProjectVariable[] Variables { get; set; } = variables;
    public TenantProjectVariable[] MissingVariables { get; set; } = null;
}
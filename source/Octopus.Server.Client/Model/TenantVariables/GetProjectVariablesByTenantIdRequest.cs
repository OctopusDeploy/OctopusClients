namespace Octopus.Client.Model.TenantVariables;

public class GetProjectVariablesByTenantIdRequest(string tenantId, string spaceId)
{
    public string TenantId { get; set; } = tenantId;

    public string SpaceId { get; set; } = spaceId;
}

public class GetProjectVariablesByTenantIdResponse(string tenantId, TenantProjectVariable[] projectVariables)
{
    public string TenantId { get; set; } = tenantId;

    public TenantProjectVariable[] ProjectVariables { get; set; } = projectVariables;
}
namespace Octopus.Client.Model.TenantVariables;

public class GetCommonVariablesByTenantIdRequest(string tenantId, string spaceId)
{
    public string TenantId { get; set; } = tenantId;

    public string SpaceId { get; set; } = spaceId;
}

public class GetCommonVariablesByTenantIdResponse(string tenantId, TenantCommonVariable[] variables)
{
    public string TenantId { get; set; } = tenantId;

    public TenantCommonVariable[] Variables { get; set; } = variables;
}
﻿namespace Octopus.Client.Model.TenantVariables;

public class GetCommonVariablesByTenantIdRequest(string tenantId, string spaceId)
{
    public string TenantId { get; set; } = tenantId;

    public string SpaceId { get; set; } = spaceId;
    
    public bool IncludeMissingVariables { get; set; } = false;
}

public class GetCommonVariablesByTenantIdResponse(string tenantId, TenantCommonVariable[] commonVariables)
{
    public string TenantId { get; set; } = tenantId;

    public TenantCommonVariable[] CommonVariables { get; set; } = commonVariables;
    public TenantCommonVariable[] MissingCommonVariables { get; set; } = null;
}
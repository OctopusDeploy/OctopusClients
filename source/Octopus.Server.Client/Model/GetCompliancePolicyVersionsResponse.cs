namespace Octopus.Client.Model;

public class GetCompliancePolicyVersionsResponse
{
    public string ItemType { get; }
    public int TotalResults { get; }
    public int ItemsPerPage { get; }
    public CompliancePolicyVersionResource[] Items { get; }

    public GetCompliancePolicyVersionsResponse(string itemType, int totalResults, int itemsPerPage, CompliancePolicyVersionResource[] items)
    {
        ItemType = itemType;
        TotalResults = totalResults;
        ItemsPerPage = itemsPerPage;
        Items = items;
    }
}

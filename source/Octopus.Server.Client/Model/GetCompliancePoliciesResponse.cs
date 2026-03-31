namespace Octopus.Client.Model;

public class GetCompliancePoliciesResponse
{
    public CompliancePolicyResource[] Policies { get; set; }
    public int ItemsPerPage { get; set; }
    public int FilteredItemsCount { get; set; }
    public int TotalItemsCount { get; set; }
}

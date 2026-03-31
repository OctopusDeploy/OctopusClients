namespace Octopus.Client.Model;

public class GetCompliancePolicyVersionsRequest
{
    public string Slug { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}

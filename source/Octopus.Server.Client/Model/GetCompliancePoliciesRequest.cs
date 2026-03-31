namespace Octopus.Client.Model;

public class GetCompliancePoliciesRequest
{
    public string GitRef { get; set; }
    public string PartialName { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}

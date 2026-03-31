using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model;

public class CompliancePolicyResource
{
    [WriteableOnCreate]
    public string GitRef { get; }
    
    [WriteableOnCreate]
    public string Slug { get; }
    
    [Writeable]
    public string Name { get; set; }
    
    [Writeable]
    public string Description { get; set; }
    
    [Writeable]
    public string ScopeRego { get; set; }
    
    [Writeable]
    public string ConditionsRego { get; set; }
    
    [Writeable]
    public string ViolationReason { get; set; }
    
    [Writeable]
    public string ViolationAction { get; set; }

    public CompliancePolicyResource(string gitRef, string slug)
    {
        GitRef = gitRef;
        Slug = slug;
    }
}

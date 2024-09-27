namespace Octopus.Client.Model;

public class CommonVariableValueResource
{
    public string TemplateId { get; set; }

    public PropertyValueResource Value { get; set; }

    public string[] Scope { get; set; }
}
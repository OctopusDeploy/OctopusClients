using Octopus.TinyTypes;

namespace Octopus.Client.Model;

public class RunbookRetentionUnit(string value) : CaseInsensitiveStringTinyType(value)
{
    public static RunbookRetentionUnit Days => new("Days");
    public static RunbookRetentionUnit Items => new("Items");
}
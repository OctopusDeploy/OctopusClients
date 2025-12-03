using Octopus.TinyTypes;

namespace Octopus.Client.Model;

public class RunbookRetentionUnit : CaseInsensitiveStringTinyType
{
    internal RunbookRetentionUnit(string value) : base(value)
    {
    }

    public static RunbookRetentionUnit Days => new("Days");
    public static RunbookRetentionUnit Items => new("Items");
}

namespace Octopus.Client.Model;

public class ModifyRunbookProcessCommand : RunbookProcessResource, ICommitCommand
{
    public string ChangeDescription { get; set; }
}
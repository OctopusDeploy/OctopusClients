namespace Octopus.Client.Model;

public class DeleteRunbookCommand: RunbookResource, ICommitCommand
{
    public string ChangeDescription { get; set; }
}
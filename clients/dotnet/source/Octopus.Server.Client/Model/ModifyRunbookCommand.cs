namespace Octopus.Client.Model
{
    public class ModifyRunbookCommand : RunbookResource, ICommitCommand
    {
        public string ChangeDescription { get; set; }
    }
}
namespace Octopus.Client.Model
{
    public class ModifyChannelCommand : ChannelResource, ICommitCommand
    {
        public string ChangeDescription { get; set; }
    }
}
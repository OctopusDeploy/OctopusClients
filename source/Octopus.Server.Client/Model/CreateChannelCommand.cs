namespace Octopus.Client.Model
{
    public class CreateChannelCommand : ChannelResource, ICommitCommand
    {
        public string ChangeDescription { get; set; }
    }
}
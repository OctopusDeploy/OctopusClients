namespace Octopus.Client.Model
{
    public interface ICommitCommand
    {
        string ChangeDescription { get; set; }
    }
}
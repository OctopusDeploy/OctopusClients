namespace Octopus.Client.Model.IssueTrackers
{
    public class WorkItemLink
    {
        public string Id { get; set; }
        public string LinkUrl { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
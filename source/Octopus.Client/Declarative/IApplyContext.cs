namespace Octopus.Client.Declarative
{
    public interface IApplyContext
    {
        ApplyAction Action { get; }
        void ReportDifference(IDeclarativeResource resource, string reason);
    }
}
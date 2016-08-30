
namespace Octopus.Client.Model
{
    public enum MachineConnectivityBehavior
    {
        ExpectedToBeOnline,
        MayBeOfflineAndCanBeSkipped
    }

    public class MachineConnectivityPolicy
    {
        public MachineConnectivityBehavior MachineConnectivityBehavior { get; set; }
    }
}

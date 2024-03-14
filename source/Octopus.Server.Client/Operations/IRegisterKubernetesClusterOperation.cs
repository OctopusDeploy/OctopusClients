namespace Octopus.Client.Operations;

public interface IRegisterKubernetesClusterOperation : IRegisterMachineOperation
{
    string DefaultNamespace { get; set; }
}
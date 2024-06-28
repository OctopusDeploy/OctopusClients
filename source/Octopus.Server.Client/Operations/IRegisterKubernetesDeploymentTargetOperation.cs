namespace Octopus.Client.Operations;

public interface IRegisterKubernetesDeploymentTargetOperation : IRegisterMachineOperation
{
    string DefaultNamespace { get; set; }
}
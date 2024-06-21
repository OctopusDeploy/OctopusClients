using System;

namespace Octopus.Client.Operations;

[Obsolete($"Use {nameof(IRegisterKubernetesDeploymentTargetOperation)} instead.")]
public interface IRegisterKubernetesClusterOperation : IRegisterMachineOperation
{
    string DefaultNamespace { get; set; }
}
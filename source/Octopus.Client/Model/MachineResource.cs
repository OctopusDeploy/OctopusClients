using System;

namespace Octopus.Client.Model
{
    [Obsolete("MachineResource was deprecated in Octopus 2018.5.0.  Please use DeploymentTargetResource instead.")]
    public class MachineResource : DeploymentTargetResource
    {
        public MachineResource() : base()
        {

        }
    }
}
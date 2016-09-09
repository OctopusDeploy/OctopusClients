using System;

namespace Octopus.Client.Model.Endpoints
{
    public class OfflineDropEndpointResource : AgentlessEndpointResource
    {
        public OfflineDropEndpointResource()
        {
            SensitiveVariablesEncryptionPassword = new SensitiveValue();
        }

        public override CommunicationStyle CommunicationStyle
        {
            get { return CommunicationStyle.OfflineDrop; }
        }

        [Trim]
        [Writeable]
        public string DropFolderPath { get; set; }

        [Writeable]
        public SensitiveValue SensitiveVariablesEncryptionPassword { get; set; }

        [Trim]
        [Writeable]
        public string ApplicationsDirectory { get; set; }

        [Trim]
        [Writeable]
        public string OctopusWorkingDirectory { get; set; }
    }
}
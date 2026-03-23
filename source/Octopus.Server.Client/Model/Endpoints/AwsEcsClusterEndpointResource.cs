using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class AwsEcsClusterEndpointResource : EndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.AwsEcsCluster;

        [Trim]
        [Writeable]
        public string DefaultWorkerPoolId { get; set; }

        [Trim]
        [Writeable]
        public string ClusterName { get; set; } = string.Empty;

        [Trim]
        [Writeable]
        public string Region { get; set; } = string.Empty;

        [Trim]
        [Writeable]
        public string AccountId { get; set; } = string.Empty;

        [Trim]
        [Writeable]
        public bool UseInstanceRole { get; set; } = true;

        [Trim]
        [Writeable]
        public bool AssumeRole { get; set; } = false;

        [Trim]
        [Writeable]
        public string AssumedRoleArn { get; set; }

        [Trim]
        [Writeable]
        public string AssumedRoleSession { get; set; }

        [Trim]
        [Writeable]
        public int? AssumeRoleSessionDurationSeconds { get; set; }

        [Trim]
        [Writeable]
        public string AssumeRoleExternalId { get; set; }
    }
}

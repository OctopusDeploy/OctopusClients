namespace Octopus.Client.Model
{
    public enum FeedType
    {
        None = 0,
        NuGet,
        Docker,
        Maven,
        OctopusProject,
        GitHub,
        BuiltIn,
        Helm,
        OciRegistry,
        AwsElasticContainerRegistry,
        S3,
        AzureContainerRegistry,
        GoogleContainerRegistry,
        ArtifactoryGeneric,
        Artifact
    }
}
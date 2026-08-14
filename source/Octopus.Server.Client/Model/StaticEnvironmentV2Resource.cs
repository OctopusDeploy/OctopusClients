namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents a Static environment as returned by the GET environments/v2 endpoints.
    /// Environments are user-defined and map to real world deployment environments
    /// such as development, staging, test and production. Projects are deployed to environments.
    /// </summary>
    class StaticEnvironmentV2Resource : BaseEnvironmentV2Resource
    {
        public bool UseGuidedFailure { get; set; }

        public bool AllowDynamicInfrastructure { get; set; }
    }
}

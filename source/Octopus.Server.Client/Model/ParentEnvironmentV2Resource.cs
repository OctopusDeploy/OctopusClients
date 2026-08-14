namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents a Parent environment as returned by the GET environments/v2 endpoints. Parent
    /// environments cannot be deployed to directly; they provide values and scoping to their
    /// child Ephemeral environments. Deployment targets can still be registered against a Parent
    /// environment, which scopes them as available for deployment to that parent's Ephemeral children.
    /// </summary>
    class ParentEnvironmentV2Resource : BaseEnvironmentV2Resource
    {
        public bool UseGuidedFailure { get; set; }
    }
}

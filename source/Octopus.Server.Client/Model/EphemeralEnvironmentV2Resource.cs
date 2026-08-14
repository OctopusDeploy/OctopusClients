namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents an Ephemeral environment as returned by the GET environments/v2 endpoints.
    /// Ephemeral environments are temporary environments which inherit scoping and other values
    /// from a Parent environment.
    /// </summary>
    class EphemeralEnvironmentV2Resource : BaseEnvironmentV2Resource
    {
        public string ParentEnvironmentId { get; set; }
    }
}

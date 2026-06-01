using Newtonsoft.Json;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model;

public class SshKnownHostResource
{
    /// <summary>
    /// Gets or sets a unique identifier for this resource.
    /// </summary>
    [JsonProperty(Order = -100, NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the host.
    /// </summary>
    [Writeable]
    [Trim]
    public string Host { get; set; }

    /// <summary>
    /// Gets or sets the key type.
    /// </summary>
    [Writeable]
    [Trim]
    public string KeyType { get; set; }

    /// <summary>
    /// Gets or sets the public key.
    /// </summary>
    [Writeable]
    [Trim]
    public string PublicKey { get; set; }
}

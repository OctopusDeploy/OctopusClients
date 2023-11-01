using Newtonsoft.Json;
using Octopus.TinyTypes;

namespace Octopus.Client.Model.Endpoints;

public class TentacleCommunicationModeResource : CaseInsensitiveStringTinyType
{
    public static TentacleCommunicationModeResource Polling => new(nameof(Polling));

    public static TentacleCommunicationModeResource Listening => new(nameof(Listening));

    [JsonConstructor]
    internal TentacleCommunicationModeResource(string value) : base(value)
    {
    }
}
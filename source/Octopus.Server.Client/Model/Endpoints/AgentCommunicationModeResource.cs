using Octopus.TinyTypes;

namespace Octopus.Client.Model.Endpoints;

public class AgentCommunicationModeResource : CaseInsensitiveStringTinyType
{
    public static AgentCommunicationModeResource Polling => new(nameof(Polling));

    public static AgentCommunicationModeResource Listening => new(nameof(Listening));

    private AgentCommunicationModeResource(string value) : base(value)
    {
    }
}
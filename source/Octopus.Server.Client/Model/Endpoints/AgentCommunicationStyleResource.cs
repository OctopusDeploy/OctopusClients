using Octopus.TinyTypes;

namespace Octopus.Client.Model.Endpoints;

public class AgentCommunicationStyleResource : CaseInsensitiveStringTinyType
{
    public static AgentCommunicationStyleResource Polling => new(nameof(Polling));

    public static AgentCommunicationStyleResource Listening => new(nameof(Listening));

    private AgentCommunicationStyleResource(string value) : base(value)
    {
    }
}
using System;
using Octopus.TinyTypes;

namespace Octopus.Client.Model.Endpoints;

public class AgentCommunicationModeResource : CaseInsensitiveStringTinyType
{
    public static AgentCommunicationModeResource Polling => new(nameof(Polling));

    public static AgentCommunicationModeResource Listening => new(nameof(Listening));

    private AgentCommunicationModeResource(string value) : base(value)
    {
    }

    public static bool TryParse(string value, out AgentCommunicationModeResource agentCommunicationMode)
    {
        if (value.Equals(Listening.Value, StringComparison.InvariantCultureIgnoreCase))
        {
            agentCommunicationMode = Listening;
            return true;
        }

        if (value.Equals(Polling.Value, StringComparison.InvariantCultureIgnoreCase))
        {
            agentCommunicationMode = Polling;
            return true;
        }

        agentCommunicationMode = default;
        return false;
    }
}
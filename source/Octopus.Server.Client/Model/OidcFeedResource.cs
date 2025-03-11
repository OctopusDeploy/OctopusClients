#nullable enable

using System.Collections.Generic;

namespace Octopus.Client.Model;

public class OidcFeedResource
{
    public OidcFeedResource(string? jwt, string? audience, IEnumerable<string> deploymentSubjectKeys, IEnumerable<string> feedTestSubjectKeys)
    {
        Jwt = jwt;
        Audience = audience;
        DeploymentSubjectKeys = deploymentSubjectKeys;
        FeedTestSubjectKeys = feedTestSubjectKeys;
    }

    public string? Jwt { get; set; }

    public string? Audience { get; set; }

    public IEnumerable<string> DeploymentSubjectKeys { get; set; }

    public IEnumerable<string> FeedTestSubjectKeys { get; set; }
}
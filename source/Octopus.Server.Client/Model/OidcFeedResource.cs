#nullable enable

using System.Collections.Generic;

namespace Octopus.Client.Model;

public interface IOidcFeedAuthentication
{
    public string? Audience { get; set; }

    public IEnumerable<string> SubjectKeys { get; set; }
}
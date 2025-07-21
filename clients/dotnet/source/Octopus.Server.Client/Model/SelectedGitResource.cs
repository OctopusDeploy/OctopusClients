﻿using Octopus.Client.Model.Git;

namespace Octopus.Client.Model;

public class SelectedGitResource
{
    protected SelectedGitResource()
    {
    }

    /// <param name="actionName">The name of the deployment action</param>
    /// <param name="gitReferenceResource">The selected Git reference</param>
    public SelectedGitResource(string actionName, GitReferenceResource gitReferenceResource)
    {
        ActionName = actionName;
        GitReferenceResource = gitReferenceResource;
    }

    /// <summary>
    /// The name of the deployment action
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// The selected Git reference
    /// </summary>
    public GitReferenceResource GitReferenceResource { get; set; }
    
    /// <summary>
    /// The name of the Git resource reference
    /// </summary>
    /// <remarks>May be empty for steps which have a primary Git resource</remarks>
    public string GitResourceReferenceName { get; set; } = "";
}

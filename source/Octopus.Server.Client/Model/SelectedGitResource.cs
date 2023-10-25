using Octopus.Client.Model.Git;

namespace Octopus.Client.Model;

public class SelectedGitResource
{
    protected SelectedGitResource()
    {
    }

    /// <param name="actionName">The name of the deployment action</param>
    /// <param name="gitReferenceName">The name to use for the git reference</param>
    /// <param name="gitReferenceResource">The selected Git reference</param>
    public SelectedGitResource(string actionName, string gitReferenceName, GitReferenceResource gitReferenceResource)
    {
        ActionName = actionName;
        GitReferenceName = gitReferenceName;
        GitReferenceResource = gitReferenceResource;
    }

    /// <summary>
    /// The name to use for the git reference.
    /// </summary>
    public string GitReferenceName { get; set; }

    /// <summary>
    /// The name of the deployment action
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// The selected Git reference
    /// </summary>
    public GitReferenceResource GitReferenceResource { get; set; }
}

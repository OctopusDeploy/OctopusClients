namespace Octopus.Client.Model;

public class RunbookRunPreviewParameters(RunbookRunPreviewTarget[] deploymentPreviews)
{
    public RunbookRunPreviewTarget[] DeploymentPreviews { get; set; } = deploymentPreviews;
    public bool IncludeDisabledSteps { get; set; }
}

public class RunbookRunPreviewTarget(string environmentId)
{

    public string EnvironmentId { get; set; } = environmentId;

    public string TenantId { get; set; }
}
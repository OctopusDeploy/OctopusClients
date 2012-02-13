using System;
using OctopusTools.Client;
using OctopusTools.Model;

// ReSharper disable CheckNamespace
public static class DeploymentExtensions
// ReSharper restore CheckNamespace
{
    public static Deployment DeployRelease(this IOctopusSession session, Release release, DeploymentEnvironment environment, bool forceRedeploymentOfExistingPackages = false)
    {
        var deployment = new Deployment();
        deployment.EnvironmentId = environment.Id;
        deployment.ReleaseId = release.Id;
        deployment.ForceRedeployment = forceRedeploymentOfExistingPackages;

        return session.Create(release.Link("Deployments"), deployment);
    }
}
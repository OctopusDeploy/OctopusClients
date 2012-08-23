using System;
using OctopusTools.Client;
using OctopusTools.Model;
using System.Collections.Generic;
using log4net;

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

    public static IEnumerable<string> RequestDeployments(this IOctopusSession session, Release release, IEnumerable<DeploymentEnvironment> environments, bool force, ILog log)
    {
        var linksToDeploymentTasks = new List<string>();
        foreach (var environment in environments)
        {
            var deployment = session.DeployRelease(release, environment, force);
            var linkToTask = deployment.Link("Task");
            linksToDeploymentTasks.Add(linkToTask);

            log.InfoFormat("Successfully scheduled release '{0}' for deployment to environment '{1}'" + deployment.Name, release.Version, environment.Name);
        }
        return linksToDeploymentTasks;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using OctopusTools.Client;
using OctopusTools.Model;

// ReSharper disable CheckNamespace
public static class EnvironmentExtensions
// ReSharper restore CheckNamespace
{
    public static IList<DeploymentEnvironment> ListEnvironments(this IOctopusSession session)
    {
        return session.List<DeploymentEnvironment>(session.RootDocument.Link("Environments"));
    }

    public static IList<DeploymentEnvironment> FindEnvironments(this IOctopusSession session, ICollection<string> environmentNames)
    {
        if (environmentNames == null || !environmentNames.Any())
            return new List<DeploymentEnvironment>();

        var list = new List<DeploymentEnvironment>();
        var environments = session.List<DeploymentEnvironment>(session.RootDocument.Link("Environments"));

        foreach (var environmentName in environmentNames)
        {
            if (string.IsNullOrWhiteSpace(environmentName))
                continue;

            var environment = environments.FirstOrDefault(x => string.Equals(x.Name, environmentName, StringComparison.InvariantCultureIgnoreCase));
            if (environment == null)
            {
                throw new ArgumentException(string.Format("An environment named '{0}' could not be found.", environmentName));
            }

            list.Add(environment);
        }

        return list;
    }
}
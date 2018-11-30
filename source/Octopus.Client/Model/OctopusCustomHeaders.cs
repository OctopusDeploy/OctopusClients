using System;
using Octopus.Client.AutomationEnvironments;
using Octopus.Client.Extensions;

namespace Octopus.Client.Model
{
    internal class OctopusCustomHeaders
    {
        internal static IAutomationEnvironmentProvider automationEnvironmentProvider = new AutomationEnvironmentProvider();

        internal OctopusCustomHeaders(string requestingTool = null)
        {
            AutomationEnvironment = automationEnvironmentProvider.DetermineAutomationEnvironment();
            var automationContext = AutomationEnvironment.ToString();
            if (!string.IsNullOrWhiteSpace(requestingTool))
            {
                automationContext += $"/{requestingTool}";
            }

            var version = typeof(OctopusCustomHeaders).GetSemanticVersion();

            UserAgent = $"{ApiConstants.OctopusUserAgentProductName}/{version.ToNormalizedString()} {automationContext}";
        }

        internal string UserAgent { get; }
        internal AutomationEnvironment AutomationEnvironment { get; }
    }
}
using Octopus.Client.AutomationEnvironments;
using Octopus.Client.Extensions;

namespace Octopus.Client.Model
{
    internal class OctopusCustomHeaders
    {
        internal static IEnvironmentHelper environmentHelper = new EnvironmentHelper();
        internal static IAutomationEnvironmentProvider automationEnvironmentProvider = new AutomationEnvironmentProvider();

        internal OctopusCustomHeaders(string requestingTool = null)
        {
            var systemInformation = string.Join("; ", environmentHelper.SafelyGetEnvironmentInformation());

            AutomationEnvironment = automationEnvironmentProvider.DetermineAutomationEnvironment();
            var automationContext = AutomationEnvironment.ToString();
            if (!string.IsNullOrWhiteSpace(requestingTool))
            {
                automationContext += $" {requestingTool}";
            }

            var version = typeof(OctopusCustomHeaders).GetSemanticVersion();

            UserAgent = $"{ApiConstants.OctopusUserAgentProductName}/{version.ToNormalizedString()} ({systemInformation}) {automationContext}";
        }

        internal string UserAgent { get; }
        internal AutomationEnvironment AutomationEnvironment { get; }
    }
}
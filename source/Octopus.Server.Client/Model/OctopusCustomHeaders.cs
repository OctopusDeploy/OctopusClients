using Octopus.Client.AutomationEnvironments;
using Octopus.Client.Extensions;

namespace Octopus.Client.Model
{
    internal class OctopusCustomHeaders
    {
        internal static IEnvironmentHelper environmentHelper = new EnvironmentHelper();
        internal static IAutomationEnvironmentProvider automationEnvironmentProvider = new AutomationEnvironmentProvider();
        internal static IAiAgentProvider aiAgentProvider = new AiAgentProvider();

        internal OctopusCustomHeaders(string requestingTool = null)
        {
            var systemInformation = string.Join("; ", environmentHelper.SafelyGetEnvironmentInformation());

            var automationContext = automationEnvironmentProvider.DetermineAutomationEnvironmentWithVersion();
            if (!string.IsNullOrWhiteSpace(requestingTool))
            {
                automationContext += $" {requestingTool}";
            }

            var version = typeof(OctopusCustomHeaders).GetSemanticVersion();

            UserAgent = $"{ApiConstants.OctopusUserAgentProductName}/{version.ToNormalizedString()} ({systemInformation}) {automationContext}";

            // Appended last so everything before it is unchanged, and parsers that match on the start of the header keep working.
            var aiAgent = aiAgentProvider.DetermineAiAgent();
            if (!string.IsNullOrWhiteSpace(aiAgent))
            {
                UserAgent += $" {aiAgent}";
            }
        }

        internal string UserAgent { get; }
    }
}

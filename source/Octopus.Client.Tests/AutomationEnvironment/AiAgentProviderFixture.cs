using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.AutomationEnvironments;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.AutomationEnvironments
{
    public class AiAgentProviderFixture
    {
        [TearDown]
        public void TearDown()
        {
            AutomationEnvironmentProvider.environmentVariableReader = new EnvironmentVariableReader();
            OctopusCustomHeaders.environmentHelper = new EnvironmentHelper();
        }

        [Test]
        public void NoAgentIsDetectedWhenNoVariablesAreSet()
        {
            DetermineAiAgent().Should().BeNull();
        }

        [Test]
        public void AgentIsDetectedFromAVendorVariable()
        {
            DetermineAiAgent(("CLAUDECODE", "1")).Should().Be("(ai-agent=ClaudeCode)");
        }

        [TestCase("CURSOR_EXTENSION_HOST_ROLE", "agent-exec", "(ai-agent=Cursor)")]
        [TestCase("CURSOR_EXTENSION_HOST_ROLE", "user", null)]
        [TestCase("AGENT", "amp", "(ai-agent=Amp)")]
        [TestCase("AGENT", "teamcity-agent-01", null)]
        public void ValueRulesRequireAMatchingValue(string variable, string value, string expected)
        {
            DetermineAiAgent((variable, value)).Should().Be(expected);
        }

        [TestCase("CURSOR_TRACE_ID")]
        [TestCase("COPILOT_GITHUB_TOKEN")]
        public void VariablesSetWithoutAnAgentAreIgnored(string variable)
        {
            DetermineAiAgent((variable, "1")).Should().BeNull();
        }

        [Test]
        public void TheFirstKnownAgentWinsWhenAgentsAreNested()
        {
            DetermineAiAgent(("CURSOR_AGENT", "1"), ("CLAUDECODE", "1")).Should().Be("(ai-agent=ClaudeCode)");
        }

        [TestCase("claude-code_2-1-322_agent", "(ai-agent=ClaudeCode)")]
        [TestCase("Claude Code", "(ai-agent=ClaudeCode)")]
        [TestCase("github_copilot_vscode_agent", "(ai-agent=GitHubCopilot)")]
        [TestCase("devin@1", "(ai-agent=Devin)")]
        [TestCase("custom-agent@2.0", "(ai-agent=Other)")]
        [TestCase("amplitude-mcp", "(ai-agent=Other)")]
        public void AiAgentValueIsMappedToAKnownName(string value, string expected)
        {
            DetermineAiAgent((AiAgentProvider.AiAgentVariable, value)).Should().Be(expected);
        }

        [Test]
        public void AiAgentTakesPrecedenceOverVendorVariables()
        {
            DetermineAiAgent(("CURSOR_AGENT", "1"), (AiAgentProvider.AiAgentVariable, "claude-code_2-1-322_agent"))
                .Should().Be("(ai-agent=ClaudeCode)");
        }

        [Test]
        public void UnrecognisedAiAgentFallsBackToVendorName()
        {
            DetermineAiAgent(("GEMINI_CLI", "1"), (AiAgentProvider.AiAgentVariable, "gcli")).Should().Be("(ai-agent=GeminiCLI)");
        }

        [TestCase("  evil) agent\r\nX-Injected: 1 ", "(ai-agent=Other)")]
        [TestCase(" \t ", null)]
        public void RawAiAgentValueIsNeverIncluded(string value, string expected)
        {
            DetermineAiAgent((AiAgentProvider.AiAgentVariable, value)).Should().Be(expected);
        }

        [Test]
        public void UserAgentKeepsTheExistingHeaderAndAppendsTheAgent()
        {
            OctopusCustomHeaders.environmentHelper = new TestEnvironmentHelper();
            AutomationEnvironmentProvider.environmentVariableReader = new TestEnvironmentVariableReader(("GITHUB_ACTIONS", "true"));
            var withoutAgent = new OctopusCustomHeaders("octo/9.0.0").UserAgent;

            AutomationEnvironmentProvider.environmentVariableReader = new TestEnvironmentVariableReader(("GITHUB_ACTIONS", "true"), ("CLAUDECODE", "1"));
            var withAgent = new OctopusCustomHeaders("octo/9.0.0").UserAgent;

            withoutAgent.Should().EndWith("(TestOS; x64) GitHubActions octo/9.0.0");
            withAgent.Should().Be(withoutAgent + " (ai-agent=ClaudeCode)");
        }

        [Test]
        public void UserAgentIsUnchangedWhenNoAgentIsDetected()
        {
            OctopusCustomHeaders.environmentHelper = new TestEnvironmentHelper();
            AutomationEnvironmentProvider.environmentVariableReader = new TestEnvironmentVariableReader();

            var userAgent = new OctopusCustomHeaders("octo/9.0.0").UserAgent;

            userAgent.Should().EndWith("(TestOS; x64) NoneOrUnknown octo/9.0.0");
        }

        static string DetermineAiAgent(params (string Name, string Value)[] variables)
        {
            AutomationEnvironmentProvider.environmentVariableReader = new TestEnvironmentVariableReader(variables);
            return new AiAgentProvider().DetermineAiAgent();
        }

        class TestEnvironmentVariableReader : IEnvironmentVariableReader
        {
            readonly Dictionary<string, string> variables;

            public TestEnvironmentVariableReader(params (string Name, string Value)[] variables)
            {
                this.variables = variables.ToDictionary(v => v.Name, v => v.Value);
            }

            public string GetVariableValue(string name)
            {
                return variables.TryGetValue(name, out var value) ? value : null;
            }
        }

        class TestEnvironmentHelper : IEnvironmentHelper
        {
            public string[] SafelyGetEnvironmentInformation()
            {
                return new[] { "TestOS", "x64" };
            }
        }
    }
}

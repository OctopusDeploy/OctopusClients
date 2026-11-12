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

        [TestCase("CLAUDECODE", "1", "AIAgent/ClaudeCode")]
        [TestCase("CLAUDE_CODE_ENTRYPOINT", "cli", "AIAgent/ClaudeCode")]
        [TestCase("CURSOR_AGENT", "1", "AIAgent/Cursor")]
        [TestCase("CURSOR_EXTENSION_HOST_ROLE", "agent-exec", "AIAgent/Cursor")]
        [TestCase("CODEX_SANDBOX", "seatbelt", "AIAgent/Codex")]
        [TestCase("CODEX_THREAD_ID", "abc", "AIAgent/Codex")]
        [TestCase("GEMINI_CLI", "1", "AIAgent/GeminiCLI")]
        [TestCase("COPILOT_CLI", "1", "AIAgent/GitHubCopilot")]
        [TestCase("OPENCODE", "1", "AIAgent/OpenCode")]
        [TestCase("OPENCODE_CLIENT", "cli", "AIAgent/OpenCode")]
        [TestCase("ANTIGRAVITY_AGENT", "1", "AIAgent/Antigravity")]
        [TestCase("GOOSE_TERMINAL", "1", "AIAgent/Goose")]
        [TestCase("AGENT", "goose", "AIAgent/Goose")]
        [TestCase("AGENT", "amp", "AIAgent/Amp")]
        [TestCase("CLINE_ACTIVE", "true", "AIAgent/Cline")]
        [TestCase("ROO_CODE_TASK_ID", "abc", "AIAgent/RooCode")]
        [TestCase("AUGMENT_AGENT", "1", "AIAgent/Augment")]
        [TestCase("QWEN_CODE", "1", "AIAgent/QwenCode")]
        [TestCase("DROID_CLI", "1", "AIAgent/FactoryDroid")]
        [TestCase("KIMI_CLI", "1", "AIAgent/KimiCLI")]
        [TestCase("TRAE_AI_SHELL_ID", "abc", "AIAgent/Trae")]
        [TestCase("OR_APP_NAME", "aider", "AIAgent/Aider")]
        [TestCase("OR_APP_NAME", "OpenHands", "AIAgent/OpenHands")]
        public void AgentIsDetectedFromVendorVariables(string variable, string value, string expected)
        {
            DetermineAiAgent((variable, value)).Should().Be(expected);
        }

        [TestCase("CURSOR_EXTENSION_HOST_ROLE", "user")]
        [TestCase("OPENCODE", "0")]
        [TestCase("AGENT", "teamcity-agent-01")]
        [TestCase("OR_APP_NAME", "SomethingElse")]
        public void ValueRulesRequireAMatchingValue(string variable, string value)
        {
            DetermineAiAgent((variable, value)).Should().BeNull();
        }

        [TestCase("CURSOR_TRACE_ID")]
        [TestCase("COPILOT_GITHUB_TOKEN")]
        [TestCase("GOOSE_PROVIDER")]
        [TestCase("REPL_ID")]
        [TestCase("ZED_TERM")]
        public void VariablesSetWithoutAnAgentAreIgnored(string variable)
        {
            DetermineAiAgent((variable, "1")).Should().BeNull();
        }

        [Test]
        public void TheFirstKnownAgentWinsWhenAgentsAreNested()
        {
            DetermineAiAgent(("CURSOR_AGENT", "1"), ("CLAUDECODE", "1")).Should().Be("AIAgent/ClaudeCode");
        }

        [TestCase("claude-code_2-1-322_agent", "AIAgent/ClaudeCode")]
        [TestCase("claude-code", "AIAgent/ClaudeCode")]
        [TestCase("Claude-Code", "AIAgent/ClaudeCode")]
        [TestCase("Claude Code", "AIAgent/ClaudeCode")]
        [TestCase("cursor-cli", "AIAgent/Cursor")]
        [TestCase("github_copilot_vscode_agent", "AIAgent/GitHubCopilot")]
        [TestCase("github-copilot-cli", "AIAgent/GitHubCopilot")]
        [TestCase("devin@1", "AIAgent/Devin")]
        [TestCase("custom-agent@2.0", "AIAgent/Other")]
        public void AiAgentValueIsMappedToAKnownName(string value, string expected)
        {
            DetermineAiAgent((AiAgentProvider.AiAgentVariable, value)).Should().Be(expected);
        }

        [Test]
        public void AiAgentTakesPrecedenceOverVendorVariables()
        {
            DetermineAiAgent(("CURSOR_AGENT", "1"), (AiAgentProvider.AiAgentVariable, "claude-code_2-1-322_agent"))
                .Should().Be("AIAgent/ClaudeCode");
        }

        [Test]
        public void UnrecognisedAiAgentFallsBackToVendorName()
        {
            DetermineAiAgent(("GEMINI_CLI", "1"), (AiAgentProvider.AiAgentVariable, "gcli")).Should().Be("AIAgent/GeminiCLI");
        }

        [TestCase("  evil) agent\r\nX-Injected: 1 ", "AIAgent/Other")]
        [TestCase(" \t ", null)]
        public void RawAiAgentValueIsNeverIncluded(string value, string expected)
        {
            DetermineAiAgent((AiAgentProvider.AiAgentVariable, value)).Should().Be(expected);
        }

        [Test]
        public void UserAgentIncludesTheAgentBetweenTheAutomationEnvironmentAndTheRequestingTool()
        {
            OctopusCustomHeaders.environmentHelper = new TestEnvironmentHelper();
            AutomationEnvironmentProvider.environmentVariableReader = new TestEnvironmentVariableReader(("GITHUB_ACTIONS", "true"), ("CLAUDECODE", "1"));

            var userAgent = new OctopusCustomHeaders("octo/9.0.0").UserAgent;

            userAgent.Should().EndWith("(TestOS; x64) GitHubActions AIAgent/ClaudeCode octo/9.0.0");
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

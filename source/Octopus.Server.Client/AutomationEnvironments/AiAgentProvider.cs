using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Octopus.Client.AutomationEnvironments
{
    // The agent list is maintained in go-octopusdeploy and ported from there; keep in sync.
    // https://github.com/OctopusDeploy/go-octopusdeploy/blob/main/pkg/services/api/ai_agent_provider.go
    //
    // AI agents are detected separately from automation (CI) environments because the two are independent:
    // an agent commonly runs inside a CI system or on an Octopus worker, and both should be reported.
    // Detection relies on self-reported environment variables, so it is suitable for analytics but must not
    // be relied on for security decisions.
    internal class AiAgentProvider : IAiAgentProvider
    {
        // The emerging cross-vendor convention for an agent to identify itself.
        // https://github.com/vercel/vercel/tree/main/packages/detect-agent
        internal const string AiAgentVariable = "AI_AGENT";

        internal const string OtherAgentName = "Other";

        // Ordered: the first agent with a matching rule wins, so the result is deterministic when several
        // agents' variables are present (e.g. one agent running another).
        //
        // Variables that are commonly present without an agent driving the process are deliberately excluded:
        // CURSOR_TRACE_ID (any Cursor terminal), COPILOT_GITHUB_TOKEN and GOOSE_PROVIDER (user configuration),
        // REPL_ID (any Replit workspace) and ZED_TERM (any Zed terminal).
        internal static readonly KnownAiAgent[] KnownAgents =
        {
            new KnownAiAgent("ClaudeCode", Rule.AnyValue("CLAUDECODE"), Rule.AnyValue("CLAUDE_CODE_ENTRYPOINT")),
            new KnownAiAgent("Cursor", Rule.AnyValue("CURSOR_AGENT"), Rule.Exact("CURSOR_EXTENSION_HOST_ROLE", "agent-exec")),
            new KnownAiAgent("Codex", Rule.AnyValue("CODEX_SANDBOX"), Rule.AnyValue("CODEX_THREAD_ID"), Rule.AnyValue("CODEX_CI")),
            new KnownAiAgent("GeminiCLI", Rule.AnyValue("GEMINI_CLI")),
            new KnownAiAgent("GitHubCopilot", Rule.AnyValue("COPILOT_CLI"), Rule.AnyValue("COPILOT_AGENT"), Rule.AnyValue("GITHUB_COPILOT_CLI_MODE")),
            new KnownAiAgent("OpenCode", Rule.Exact("OPENCODE", "1"), Rule.AnyValue("OPENCODE_CLIENT"), Rule.AnyValue("OPENCODE_AI")),
            new KnownAiAgent("Antigravity", Rule.AnyValue("ANTIGRAVITY_AGENT")),
            new KnownAiAgent("Goose", Rule.AnyValue("GOOSE_TERMINAL"), Rule.Exact("AGENT", "goose")),
            new KnownAiAgent("Amp", Rule.Exact("AGENT", "amp")),
            new KnownAiAgent("Cline", Rule.AnyValue("CLINE_ACTIVE"), Rule.AnyValue("CLINE_TASK_ID")),
            new KnownAiAgent("RooCode", Rule.AnyValue("ROO_CODE_TASK_ID")),
            new KnownAiAgent("Augment", Rule.AnyValue("AUGMENT_AGENT")),
            new KnownAiAgent("QwenCode", Rule.AnyValue("QWEN_CODE")),
            new KnownAiAgent("FactoryDroid", Rule.AnyValue("DROID_CLI")),
            new KnownAiAgent("KimiCLI", Rule.AnyValue("KIMI_CLI")),
            new KnownAiAgent("Trae", Rule.AnyValue("TRAE_AI_SHELL_ID")),
            new KnownAiAgent("Aider", Rule.Exact("OR_APP_NAME", "Aider")),
            new KnownAiAgent("OpenHands", Rule.Exact("OR_APP_NAME", "OpenHands")),
        };

        // Maps the start of a normalised AI_AGENT value to a known agent name. Vendors don't agree on a format
        // (e.g. "claude-code_2-1-322_agent", "github_copilot_vscode_agent", "devin@1"), so values are lowercased
        // and runs of underscores and whitespace replaced with a single hyphen before matching.
        internal static readonly AgentNamePrefix[] AiAgentValuePrefixes =
        {
            new AgentNamePrefix("claude-code", "ClaudeCode"),
            new AgentNamePrefix("cursor", "Cursor"),
            new AgentNamePrefix("codex", "Codex"),
            new AgentNamePrefix("gemini", "GeminiCLI"),
            new AgentNamePrefix("github-copilot", "GitHubCopilot"),
            new AgentNamePrefix("opencode", "OpenCode"),
            new AgentNamePrefix("antigravity", "Antigravity"),
            new AgentNamePrefix("goose", "Goose"),
            new AgentNamePrefix("amp", "Amp"),
            new AgentNamePrefix("cline", "Cline"),
            new AgentNamePrefix("augment", "Augment"),
            new AgentNamePrefix("devin", "Devin"),
            new AgentNamePrefix("replit", "Replit"),
            new AgentNamePrefix("v0", "V0"),
        };

        // Shares the automation environment provider's reader so that tests which replace one replace both.
        static IEnvironmentVariableReader EnvironmentVariableReader => AutomationEnvironmentProvider.environmentVariableReader;

        /// <summary>
        /// Returns a User-Agent token naming the AI agent driving this process (e.g. "AIAgent/ClaudeCode"), or
        /// null when none is detected. Only a normalised name is reported: the raw AI_AGENT value is free-form
        /// and user-settable, so it is used for matching and never included in the output.
        /// </summary>
        public string DetermineAiAgent()
        {
            var aiAgentValue = EnvironmentVariableReader.GetVariableValue(AiAgentVariable)?.Trim();

            var name = NameFromAiAgentValue(aiAgentValue)
                // Also covers an agent that sets AI_AGENT in a format that isn't recognised.
                ?? NameFromVendorVariables()
                ?? (string.IsNullOrEmpty(aiAgentValue) ? null : OtherAgentName);

            return name == null ? null : $"AIAgent/{name}";
        }

        static string NameFromVendorVariables()
        {
            return KnownAgents
                .FirstOrDefault(agent => agent.Rules.Any(rule => rule.Matches(EnvironmentVariableReader.GetVariableValue(rule.Variable))))
                ?.Name;
        }

        static string NameFromAiAgentValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var normalised = Regex.Replace(value.ToLowerInvariant(), @"[_\s]+", "-");
            return AiAgentValuePrefixes.FirstOrDefault(p => normalised.StartsWith(p.Prefix, StringComparison.Ordinal))?.Name;
        }

        internal class KnownAiAgent
        {
            public KnownAiAgent(string name, params Rule[] rules)
            {
                Name = name;
                Rules = rules;
            }

            public string Name { get; }
            public Rule[] Rules { get; }
        }

        internal class Rule
        {
            Rule(string variable, string value)
            {
                Variable = variable;
                Value = value;
            }

            public string Variable { get; }

            // When null, any non-empty value matches; otherwise the value must match (case-insensitive).
            public string Value { get; }

            public static Rule AnyValue(string variable) => new Rule(variable, null);
            public static Rule Exact(string variable, string value) => new Rule(variable, value);

            public bool Matches(string actualValue)
            {
                if (string.IsNullOrEmpty(actualValue))
                {
                    return false;
                }

                return Value == null || string.Equals(actualValue, Value, StringComparison.OrdinalIgnoreCase);
            }
        }

        internal class AgentNamePrefix
        {
            public AgentNamePrefix(string prefix, string name)
            {
                Prefix = prefix;
                Name = name;
            }

            public string Prefix { get; }
            public string Name { get; }
        }
    }

    internal interface IAiAgentProvider
    {
        string DetermineAiAgent();
    }
}

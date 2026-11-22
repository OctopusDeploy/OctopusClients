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

        // Each agent is listed once, and can be recognised from AI_AGENT (by prefix), from its vendor variables, or
        // both. AI_AGENT is checked against the whole list before any vendor variables, so a self-reported identity
        // wins (e.g. Claude Code running inside Cursor). Within each lookup the first match wins, so the result is
        // deterministic when several agents' variables are present.
        //
        // Variables that are commonly present without an agent driving the process are deliberately excluded:
        // CURSOR_TRACE_ID (any Cursor terminal), COPILOT_GITHUB_TOKEN and GOOSE_PROVIDER (user configuration),
        // REPL_ID (any Replit workspace) and ZED_TERM (any Zed terminal).
        internal static readonly KnownAiAgent[] KnownAgents =
        {
            new KnownAiAgent("ClaudeCode", "claude-code", Rule.AnyValue("CLAUDECODE"), Rule.AnyValue("CLAUDE_CODE_ENTRYPOINT")),
            new KnownAiAgent("Cursor", "cursor", Rule.AnyValue("CURSOR_AGENT"), Rule.Exact("CURSOR_EXTENSION_HOST_ROLE", "agent-exec")),
            new KnownAiAgent("Codex", "codex", Rule.AnyValue("CODEX_SANDBOX"), Rule.AnyValue("CODEX_THREAD_ID"), Rule.AnyValue("CODEX_CI")),
            new KnownAiAgent("GeminiCLI", "gemini", Rule.AnyValue("GEMINI_CLI")),
            new KnownAiAgent("GitHubCopilot", "github-copilot", Rule.AnyValue("COPILOT_CLI"), Rule.AnyValue("COPILOT_AGENT"), Rule.AnyValue("GITHUB_COPILOT_CLI_MODE")),
            new KnownAiAgent("OpenCode", "opencode", Rule.Exact("OPENCODE", "1"), Rule.AnyValue("OPENCODE_CLIENT"), Rule.AnyValue("OPENCODE_AI")),
            new KnownAiAgent("Antigravity", "antigravity", Rule.AnyValue("ANTIGRAVITY_AGENT")),
            new KnownAiAgent("Goose", "goose", Rule.AnyValue("GOOSE_TERMINAL"), Rule.Exact("AGENT", "goose")),
            new KnownAiAgent("Amp", "amp", Rule.Exact("AGENT", "amp")),
            new KnownAiAgent("Cline", "cline", Rule.AnyValue("CLINE_ACTIVE"), Rule.AnyValue("CLINE_TASK_ID")),
            new KnownAiAgent("RooCode", aiAgentPrefix: null, Rule.AnyValue("ROO_CODE_TASK_ID")),
            new KnownAiAgent("Augment", "augment", Rule.AnyValue("AUGMENT_AGENT")),
            new KnownAiAgent("QwenCode", aiAgentPrefix: null, Rule.AnyValue("QWEN_CODE")),
            new KnownAiAgent("FactoryDroid", aiAgentPrefix: null, Rule.AnyValue("DROID_CLI")),
            new KnownAiAgent("KimiCLI", aiAgentPrefix: null, Rule.AnyValue("KIMI_CLI")),
            new KnownAiAgent("Trae", aiAgentPrefix: null, Rule.AnyValue("TRAE_AI_SHELL_ID")),
            new KnownAiAgent("Aider", aiAgentPrefix: null, Rule.Exact("OR_APP_NAME", "Aider")),
            new KnownAiAgent("OpenHands", aiAgentPrefix: null, Rule.Exact("OR_APP_NAME", "OpenHands")),
            new KnownAiAgent("Devin", "devin"),
            new KnownAiAgent("Replit", "replit"),
            new KnownAiAgent("V0", "v0"),
        };

        // Shares the automation environment provider's reader so that tests which replace one replace both.
        static IEnvironmentVariableReader EnvironmentVariableReader => AutomationEnvironmentProvider.environmentVariableReader;

        /// <summary>
        /// Returns a User-Agent comment naming the AI agent driving this process (e.g. "(ai-agent=ClaudeCode)"), or
        /// null when none is detected. Only a normalised name is reported: the raw AI_AGENT value is free-form
        /// and user-settable, so it is used for matching and never included in the output.
        /// </summary>
        public string DetermineAiAgent()
        {
            var aiAgentNameFromEnvVar = EnvironmentVariableReader.GetVariableValue(AiAgentVariable)?.Trim();

            // Vendor variables also cover an agent that sets AI_AGENT in a format that isn't recognised.
            var agent = FindByAiAgentPrefix(aiAgentNameFromEnvVar) ?? FindByVariables();
            if (agent != null)
            {
                return AiAgentComment(agent.Name);
            }

            return string.IsNullOrEmpty(aiAgentNameFromEnvVar) ? null : AiAgentComment(OtherAgentName);
        }

        // Formats the agent as an RFC 9110 comment, so it describes the request rather than claiming to be a product in the
        // User-Agent.
        static string AiAgentComment(string name) => $"(ai-agent={name})";

        static KnownAiAgent FindByAiAgentPrefix(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var normalised = Regex.Replace(value.ToLowerInvariant(), @"[_\s]+", "-");
            return KnownAgents.FirstOrDefault(agent => agent.AiAgentPrefix != null && MatchesAiAgentPrefix(normalised, agent.AiAgentPrefix));
        }

        // Whether the value starts with the prefix followed by the end of the value, or by a separator ("-", "@", ".")
        // or digit, so a short prefix such as "amp" doesn't match an unrelated identifier such as "amplitude-mcp".
        static bool MatchesAiAgentPrefix(string value, string prefix)
        {
            if (!value.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            if (value.Length == prefix.Length)
            {
                return true;
            }

            var next = value[prefix.Length];
            return next == '-' || next == '@' || next == '.' || (next >= '0' && next <= '9');
        }

        static KnownAiAgent FindByVariables()
        {
            return KnownAgents.FirstOrDefault(agent => agent.Variables.Any(rule => rule.Matches(EnvironmentVariableReader.GetVariableValue(rule.Variable))));
        }

        internal class KnownAiAgent
        {
            public KnownAiAgent(string name, string aiAgentPrefix, params Rule[] variables)
            {
                Name = name;
                AiAgentPrefix = aiAgentPrefix;
                Variables = variables;
            }

            public string Name { get; }

            // When set, matches the start of a normalised AI_AGENT value. Vendors don't agree on a format
            // (e.g. "claude-code_2-1-322_agent", "github_copilot_vscode_agent", "devin@1"), so values are lowercased
            // and runs of underscores and whitespace replaced with a single hyphen before matching.
            public string AiAgentPrefix { get; }

            // Vendor-specific environment variables that identify the agent.
            public Rule[] Variables { get; }
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
    }

    internal interface IAiAgentProvider
    {
        string DetermineAiAgent();
    }
}

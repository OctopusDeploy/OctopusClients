using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Diagnostics
{
    public enum BuildEnvironment
    {
        NoneOrUnknown,
        TeamCity,
        TeamFoundationBuild
    }

    public static class LogExtensions
    {
        static readonly Dictionary<string, string> Escapes;
        static bool serviceMessagesEnabled;
        static BuildEnvironment buildEnvironment;

        static LogExtensions()
        {
            serviceMessagesEnabled = false;
            buildEnvironment = BuildEnvironment.NoneOrUnknown;

            // As per: http://confluence.jetbrains.com/display/TCD65/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ServiceMessages
            Escapes = new Dictionary<string, string>
            {
                {"|", "||"},
                {"'", "|'"},
                {"\n", "|n"},
                {"\r", "|r"},
                {"\u0085", "|x"},
                {"\u2028", "|l"},
                {"\u2029", "|p"},
                {"[", "|["},
                {"]", "|]"}
            };
        }

        public static void EnableServiceMessages(this ILogger log)
        {
            serviceMessagesEnabled = true;
            buildEnvironment = (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BUILD_BUILDID")) && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGENT_WORKFOLDER")))
                ? string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")) ? BuildEnvironment.NoneOrUnknown : BuildEnvironment.TeamCity
                : BuildEnvironment.TeamFoundationBuild;
            log.Information("Build environment is {Environment:l}", buildEnvironment);
        }

        public static void DisableServiceMessages(this ILogger log)
        {
            serviceMessagesEnabled = false;
        }

        public static bool ServiceMessagesEnabled(this ILogger log)
        {
            return serviceMessagesEnabled;
        }

        public static bool IsVSTS(this ILogger log)
        {
            return buildEnvironment == BuildEnvironment.TeamFoundationBuild;
        }

        public static void ServiceMessage(this ILogger log, string messageName, string value)
        {
            if (!serviceMessagesEnabled)
                return;

            if (buildEnvironment == BuildEnvironment.TeamCity || buildEnvironment == BuildEnvironment.NoneOrUnknown)
            {
                log.Information("##teamcity[{MessageName:l} {Value:l}]", messageName, EscapeValue(value));
            }
            else
            {
                log.Information("{MessageName:l} {Value:l}", messageName, EscapeValue(value));
            }
        }

        public static void ServiceMessage(this ILogger log, string messageName, IDictionary<string, string> values)
        {
            if (!serviceMessagesEnabled)
                return;

            var valueSummary = string.Join(" ", values.Select(v => $"{v.Key}='{EscapeValue(v.Value)}'"));
            if (buildEnvironment == BuildEnvironment.TeamCity || buildEnvironment == BuildEnvironment.NoneOrUnknown)
            {
                log.Information("##teamcity[{MessageName:l} {ValueSummary:l}]", messageName, valueSummary);
            }
            else
            {
                log.Information("{MessageName:l} {ValueSummary:l}", messageName, valueSummary);
            }
        }

        public static void ServiceMessage(this ILogger log, string messageName, object values)
        {
            if (!serviceMessagesEnabled)
                return;

            if (values is string)
            {
                ServiceMessage(log, messageName, values.ToString());
            }
            else
            {
                var properties = TypeDescriptor.GetProperties(values).Cast<PropertyDescriptor>();
                var valueDictionary = properties.ToDictionary(p => p.Name, p => (string) p.GetValue(values));
                ServiceMessage(log, messageName, valueDictionary);
            }
        }

        public static void TfsServiceMessage(this ILogger log, string serverBaseUrl, ProjectResource project, ReleaseResource release)
        {
            if (!serviceMessagesEnabled)
                return;
            if (buildEnvironment == BuildEnvironment.TeamFoundationBuild || buildEnvironment == BuildEnvironment.NoneOrUnknown)
            {
                var workingDirectory = Environment.GetEnvironmentVariable("SYSTEM_DEFAULTWORKINGDIRECTORY") ?? new System.IO.FileInfo(typeof(LogExtensions).GetTypeInfo().Assembly.Location).DirectoryName;
                var selflink = new Uri(new Uri(serverBaseUrl), release.Links["Web"].AsString());
                var markdown = $"[Release {release.Version} created for '{project.Name}']({selflink})";
                var markdownFile = System.IO.Path.Combine(workingDirectory, Guid.NewGuid() + ".md");

                try
                {
                    System.IO.File.WriteAllText(markdownFile, markdown);
                }
                catch (UnauthorizedAccessException uae)
                {
                    throw new UnauthorizedAccessException($"Could not write the TFS service message file '{markdownFile}'. Please make sure the SYSTEM_DEFAULTWORKINGDIRECTORY environment variable is set to a writeable directory. If this command is not being run on a build agent, ommit the --enableservicemessages parameter.", uae);
                }

                log.Information("##vso[task.addattachment type=Distributedtask.Core.Summary;name=Octopus Deploy;]{MarkdownFile:l}", markdownFile);
            }
        }

        static string EscapeValue(string value)
        {
            if (value == null)
                return string.Empty;

            return Escapes.Aggregate(value, (current, escape) => current.Replace(escape.Key, escape.Value));
        }
    }
}

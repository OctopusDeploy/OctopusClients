using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using log4net;
using Octopus.Client.Model;

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

        public static void EnableServiceMessages(this ILog log)
        {
            serviceMessagesEnabled = true;
            buildEnvironment = (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BUILD_BUILDID")) && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGENT_WORKFOLDER")))
                ? string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION")) ? BuildEnvironment.NoneOrUnknown : BuildEnvironment.TeamCity
                : BuildEnvironment.TeamFoundationBuild;
            log.InfoFormat("Build environment is {0}", buildEnvironment);
        }

        public static void DisableServiceMessages(this ILog log)
        {
            serviceMessagesEnabled = false;
        }

        public static bool ServiceMessagesEnabled(this ILog log)
        {
            return serviceMessagesEnabled;
        }

        public static bool IsVSTS(this ILog log)
        {
            return buildEnvironment == BuildEnvironment.TeamFoundationBuild;
        }

        public static void ServiceMessage(this ILog log, string messageName, string value)
        {
            if (!serviceMessagesEnabled)
                return;

            if (buildEnvironment == BuildEnvironment.TeamCity || buildEnvironment == BuildEnvironment.NoneOrUnknown)
            {
                log.Info($"##teamcity[{messageName} {EscapeValue(value)}]");
            }
            else
            {
                log.Info($"{messageName} {EscapeValue(value)}");
            }
        }

        public static void ServiceMessage(this ILog log, string messageName, IDictionary<string, string> values)
        {
            if (!serviceMessagesEnabled)
                return;

            var valueSummary = string.Join(" ", values.Select(v => $"{v.Key}='{EscapeValue(v.Value)}'"));
            if (buildEnvironment == BuildEnvironment.TeamCity || buildEnvironment == BuildEnvironment.NoneOrUnknown)
            {
                log.Info($"##teamcity[{messageName} {valueSummary}]");
            }
            else
            {
                log.Info($"{messageName} {valueSummary}");
            }
        }

        public static void ServiceMessage(this ILog log, string messageName, object values)
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

        public static void TfsServiceMessage(this ILog log, string serverBaseUrl, ProjectResource project, ReleaseResource release)
        {
            if (!serviceMessagesEnabled)
                return;
            if (buildEnvironment == BuildEnvironment.TeamFoundationBuild || buildEnvironment == BuildEnvironment.NoneOrUnknown)
            {
                var workingDirectory = Environment.GetEnvironmentVariable("SYSTEM_DEFAULTWORKINGDIRECTORY") ?? new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                var selflink = new Uri(new Uri(serverBaseUrl), release.Links["Web"].AsString());
                var markdown = string.Format("[Release {0} created for '{1}']({2})", release.Version, project.Name, selflink);
                var markdownFile = System.IO.Path.Combine(workingDirectory, Guid.NewGuid() + ".md");
                System.IO.File.WriteAllText(markdownFile, markdown);
                log.InfoFormat("##vso[task.addattachment type=Distributedtask.Core.Summary;name=Octopus Deploy;]{0}", markdownFile);
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
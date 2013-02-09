using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using log4net;

namespace OctopusTools.Diagnostics
{
    public static class LogExtensions
    {
        static readonly Dictionary<string, string> Escapes;
        static bool serviceMessagesEnabled;

        static LogExtensions()
        {
            serviceMessagesEnabled = false;

            // As per: http://confluence.jetbrains.com/display/TCD65/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ServiceMessages
            Escapes = new Dictionary<string, string>()
            {
                { "'", "|'" },
                { "\n", "|n" },
                { "\r", "|r" },
                { "\u0085", "|x" },
                { "\u2028", "|l" },
                { "\u2029", "|p" },
                { "|", "||" },
                { "[", "|[" },
                { "]", "|]" }
            };
        }

        public static void EnableServiceMessages(this ILog log)
        {
            serviceMessagesEnabled = true;
        }

        public static void DisableServiceMessages(this ILog log)
        {
            serviceMessagesEnabled = false;
        }

        public static void ServiceMessage(this ILog log, string messageName, string value)
        {
            if (!serviceMessagesEnabled) 
                return;

            log.InfoFormat(
                "##teamcity[{0} {1}]",
                messageName,
                EscapeValue(value));
        }

        public static void ServiceMessage(this ILog log, string messageName, IDictionary<string, string> values)
        {
            if (!serviceMessagesEnabled) 
                return;

            log.InfoFormat(
                "##teamcity[{0} {1}]",
                messageName,
                string.Join(" ", values.Select(v => v.Key + "='" + EscapeValue(v.Value) + "'")));
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
                var valueDictionary = properties.ToDictionary(p => p.Name, p => (string)p.GetValue(values));
                ServiceMessage(log, messageName, valueDictionary);
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
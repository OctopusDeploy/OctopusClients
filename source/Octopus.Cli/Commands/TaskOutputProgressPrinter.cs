using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public class TaskOutputProgressPrinter
    {
        readonly HashSet<string> printed = new HashSet<string>();

        public void Render(IOctopusRepository repository, ILog log, TaskResource resource)
        {
            var details = repository.Tasks.GetDetails(resource);

            if (details.ActivityLogs != null)
            {
                foreach (var item in details.ActivityLogs)
                {
                    if (log.ServiceMessagesEnabled())
                    {
                        if (log.IsVSTS())
                            RenderToVSTS(item, log, "");
                        else
                            RenderToTeamCity(item, log);
                    }
                    else
                    {
                        RenderToConsole(item, log, "");                        
                    }
                }
            }
        }

        bool IsPrintable(ActivityElement element)
        {
            if (element.Status == ActivityStatus.Pending || element.Status == ActivityStatus.Running)
                return false;

            if (printed.Contains(element.Id))
                return false;

            printed.Add(element.Id);
            return true;
        }

        void RenderToConsole(ActivityElement element, ILog log, string indent)
        {
            if (!IsPrintable(element))
                return;

            if (element.Status == ActivityStatus.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (element.Status == ActivityStatus.SuccessWithWarning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (element.Status == ActivityStatus.Failed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine("{0}         {1}: {2}", indent, element.Status, element.Name);
            Console.ResetColor();

            foreach (var logEntry in element.LogElements)
            {
                if (logEntry.Category == "Error" || logEntry.Category == "Fatal")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (logEntry.Category == "Warning")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                log.InfoFormat("{0}{1,-8}   {2}", indent, logEntry.Category, LineSplitter.Split(indent + new string(' ', 11), logEntry.MessageText));
                Console.ResetColor();
            }

            foreach (var child in element.Children)
            {
                RenderToConsole(child, log, indent + "  ");
            }
        }

        void RenderToTeamCity(ActivityElement element, ILog log)
        {
            if (!IsPrintable(element))
                return;

            var blockName = element.Status + ": " + element.Name;

            log.ServiceMessage("blockOpened", new { name = blockName });

            foreach (var logEntry in element.LogElements)
            {
                var lines = logEntry.MessageText.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
                foreach (var line in lines)
                {
                    log.ServiceMessage("message", new { text = line, status = ConvertToTeamCityMessageStatus(logEntry.Category) });
                }
            }

            foreach (var child in element.Children)
            {
                RenderToTeamCity(child, log);
            }

            log.ServiceMessage("blockClosed", new { name = blockName });
        }

        void RenderToVSTS(ActivityElement element, ILog log, string indent)
        {
            if (!IsPrintable(element))
                return;

            log.Info($"{indent}         {element.Status}: {element.Name}");

            foreach (var logEntry in element.LogElements)
            {
                log.Info($"{logEntry.Category,-8}{indent}   {logEntry.MessageText}");
            }

            foreach (var child in element.Children)
            {
                RenderToVSTS(child, log, indent + "  ");
            }
        }

        static string ConvertToTeamCityMessageStatus(string category)
        {
            switch (category)
            {
                case "Error": return "ERROR";
                case "Fatal": return "FAILURE";
                case "Warning": return "WARNING";
            }
            return "NORMAL";
        }
    }
}
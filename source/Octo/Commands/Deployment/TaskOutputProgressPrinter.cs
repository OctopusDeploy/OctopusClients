using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Deployment
{
    public class TaskOutputProgressPrinter
    {
        readonly HashSet<string> printed = new HashSet<string>();

        public async Task Render(IOctopusAsyncRepository repository, ICommandOutputProvider commandOutputProvider, TaskResource resource)
        {
            var details = await repository.Tasks.GetDetails(resource).ConfigureAwait(false);

            if (details.ActivityLogs != null)
            {
                foreach (var item in details.ActivityLogs.SelectMany(a => a.Children))
                {
                    if (commandOutputProvider.ServiceMessagesEnabled())
                    {
                        if (commandOutputProvider.IsVSTS())
                            RenderToVSTS(item, commandOutputProvider, String.Empty);
                        else
                            RenderToTeamCity(item, commandOutputProvider);
                    }
                    else
                    {
                        RenderToConsole(item, commandOutputProvider, string.Empty);                        
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

        void RenderToConsole(ActivityElement element, ICommandOutputProvider commandOutputProvider, string indent)
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

                Console.WriteLine("{0}{1,-8}   {2}", indent, logEntry.Category, LineSplitter.Split(indent + new string(' ', 11), logEntry.MessageText));
                Console.ResetColor();
            }

            foreach (var child in element.Children)
            {
                RenderToConsole(child, commandOutputProvider, indent + "  ");
            }
        }

        void RenderToTeamCity(ActivityElement element, ICommandOutputProvider commandOutputProvider)
        {
            if (!IsPrintable(element))
                return;

            var blockName = element.Status + ": " + element.Name;

            commandOutputProvider.ServiceMessage("blockOpened", new { name = blockName });

            foreach (var logEntry in element.LogElements)
            {
                var lines = logEntry.MessageText.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
                foreach (var line in lines)
                {
                    commandOutputProvider.ServiceMessage("message", new { text = line, status = ConvertToTeamCityMessageStatus(logEntry.Category) });
                }
            }

            foreach (var child in element.Children)
            {
                RenderToTeamCity(child, commandOutputProvider);
            }

            commandOutputProvider.ServiceMessage("blockClosed", new { name = blockName });
        }

        void RenderToVSTS(ActivityElement element, ICommandOutputProvider commandOutputProvider, string indent)
        {
            if (!IsPrintable(element))
                return;

            commandOutputProvider.Information("{Indent:l}         {Status:l}: {Name:l}", indent, element.Status, element.Name);

            foreach (var logEntry in element.LogElements)
            {
                commandOutputProvider.Information("{Category,-8:l}{Indent:l}   {Message:l}", logEntry.Category, logEntry.MessageText);
            }

            foreach (var child in element.Children)
            {
                RenderToVSTS(child, commandOutputProvider, indent + "  ");
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
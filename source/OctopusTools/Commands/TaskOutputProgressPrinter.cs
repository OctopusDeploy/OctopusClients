using System;
using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusTools.Util;

namespace OctopusTools.Commands
{
    public class TaskOutputProgressPrinter
    {
        readonly HashSet<string> printed = new HashSet<string>();

        public void Render(IOctopusRepository repository, TaskResource resource)
        {
            var details = repository.Tasks.GetDetails(resource);

            if (details.ActivityLog != null)
            {
                foreach (var item in details.ActivityLog.Children)
                {
                    Render(item, "");
                }
            }
        }

        void Render(ActivityElement element, string indent)
        {
            if (element.Status == ActivityStatus.Pending || element.Status == ActivityStatus.Running)
                return;

            if (printed.Contains(element.Id))
                return;

            printed.Add(element.Id);

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
                Render(child, indent + "  ");
            }
        }
    }
}
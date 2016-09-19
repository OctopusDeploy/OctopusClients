using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;

namespace Octopus.Cli.Tests.Commands
{
    public class SpeakCommand : ICommand
    {
        public OptionSet Options
        {
            get
            {
                var options = new OptionSet();
                options.Add("message=", m => { });
                return options;
            }
        }

        public void GetHelp(TextWriter writer)
        {
            writer.WriteLine("message=");
            Options.WriteOptionDescriptions(writer);
        }

        public Task Execute(string[] commandLineArguments)
        {
            return Task.Run(() => Assert.Fail("This should not be called"));
        }
    }
}
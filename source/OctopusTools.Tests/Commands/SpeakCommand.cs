using System;
using System.IO;
using NUnit.Framework;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;

namespace OctopusTools.Tests.Commands
{
    public class SpeakCommand : ICommand
    {
        public void GetHelp(TextWriter writer)
        {
            var options = new OptionSet();
            options.Add("message=", m => { });
            options.WriteOptionDescriptions(writer);
        }

        public void Execute(string[] commandLineArguments)
        {
            Assert.Fail("This should not be called");
        }
    }
}
using System;
using System.IO;

namespace OctopusTools.Infrastructure
{
    public interface ICommand
    {
        void GetHelp(TextWriter writer);
        void Execute(params string[] commandLineArguments);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CommandAttribute : Attribute, ICommandMetadata
    {
        public CommandAttribute(string name, params string[] aliases)
        {
            Name = name;
            Aliases = aliases;
        }

        public string Name { get; set; }
        public string[] Aliases { get; set; }
        public string Description { get; set; }
    }
}

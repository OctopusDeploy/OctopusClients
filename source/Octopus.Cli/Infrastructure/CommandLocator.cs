using System;
using System.Linq;
using Autofac;

namespace Octopus.Cli.Infrastructure
{
    public class CommandLocator : ICommandLocator
    {
        readonly ILifetimeScope lifetimeScope;

        public CommandLocator(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public ICommandMetadata[] List()
        {
            return
                (from t in typeof (CommandLocator).Assembly.GetTypes()
                    where typeof (ICommand).IsAssignableFrom(t)
                    let attribute = (ICommandMetadata) t.GetCustomAttributes(typeof (CommandAttribute), true).FirstOrDefault()
                    where attribute != null
                    select attribute).ToArray();
        }

        public ICommand Find(string name)
        {
            name = name.Trim().ToLowerInvariant();
            var found = (from t in typeof (CommandLocator).Assembly.GetTypes()
                where typeof (ICommand).IsAssignableFrom(t)
                let attribute = (ICommandMetadata) t.GetCustomAttributes(typeof (CommandAttribute), true).FirstOrDefault()
                where attribute != null
                where attribute.Name == name || attribute.Aliases.Any(a => a == name)
                select t).FirstOrDefault();

            return found == null ? null : (ICommand) lifetimeScope.Resolve(found);
        }
    }
}
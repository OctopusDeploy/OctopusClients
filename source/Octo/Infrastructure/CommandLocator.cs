using System.Linq;
using System.Reflection;
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
            var iCommandType = typeof(ICommand).GetTypeInfo();
            return
            (from t in typeof(CommandLocator).GetTypeInfo().Assembly.GetTypes()
                where iCommandType.IsAssignableFrom(t)
                let attribute =
                (ICommandMetadata) t.GetTypeInfo().GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault()
                where attribute != null
                select attribute).ToArray();
        }

        public ICommand Find(string name)
        {
            name = name.Trim().ToLowerInvariant();
            var iCommandType = typeof(ICommand).GetTypeInfo();

            var found = (from t in typeof(CommandLocator).GetTypeInfo().Assembly.GetTypes()
                where iCommandType.IsAssignableFrom(t)
                let attribute =
                (ICommandMetadata) t.GetTypeInfo().GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault()
                where attribute != null
                where attribute.Name == name || attribute.Aliases.Any(a => a == name)
                select t).FirstOrDefault();

            return found == null ? null : (ICommand) lifetimeScope.Resolve(found);
        }
    }
}
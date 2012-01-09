using System;
using Autofac;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    public class CommandModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<CommandProcessor>().As<ICommandProcessor>();
            builder.RegisterType<CommandLocator>().As<ICommandLocator>();
        }
    }
}
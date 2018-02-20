using Autofac;
using Serilog;

namespace Octopus.Cli.Diagnostics
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(Log.Logger).As<ILogger>().SingleInstance();
        }
    }
}
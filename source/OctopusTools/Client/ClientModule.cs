using System;
using System.Collections.Generic;
using Autofac;
using OctopusTools.Infrastructure;

namespace OctopusTools.Client
{
    public class ClientModule : Module
    {
        readonly IEnumerable<string> args;

        public ClientModule(IEnumerable<string> args)
        {
            this.args = args;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<OctopusSessionFactory>().As<IOctopusSessionFactory>();
            builder.RegisterType<CommandLineArgsProvider>().AsImplementedInterfaces().WithParameter(new NamedParameter("args", args));
        }
    }
}
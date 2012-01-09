using System;
using Autofac;

namespace OctopusTools.Client
{
    public class ClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<HttpClientWrapper>().As<IHttpClient>();
            builder.RegisterType<OctopusClientFactory>().As<IOctopusClientFactory>();
        }
    }
}
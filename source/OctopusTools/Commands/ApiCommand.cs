using System;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public abstract class ApiCommand : ICommand
    {
        readonly IOctopusSession client;
        readonly ILog log;

        protected ApiCommand(IOctopusSession client, ILog log)
        {
            this.log = log;
            this.client = client;
        }

        protected ILog Log
        {
            get { return log; }
        }

        protected IOctopusSession Session
        {
            get { return client; }
        }

        protected RootDocument ServiceRoot
        {
            get { return client.RootDocument; }
        }

        public virtual OptionSet Options
        {
            get { return new OptionSet(); }
        }

        public abstract void Execute();
      
    }
}
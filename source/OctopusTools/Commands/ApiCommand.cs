using System;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public abstract class ApiCommand : ICommand
    {
        readonly Lazy<IOctopusSession> session;
        readonly ILog log;

        protected ApiCommand(IOctopusSessionFactory client, ILog log)
        {
            this.log = log;

            session = new Lazy<IOctopusSession>(client.OpenSession);
        }

        protected ILog Log
        {
            get { return log; }
        }

        protected IOctopusSession Session
        {
            get { return session.Value; }
        }

        protected RootDocument ServiceRoot
        {
            get { return Session.RootDocument; }
        }

        public virtual OptionSet Options
        {
            get { return new OptionSet(); }
        }

        public abstract void Execute();
      
        protected virtual void NotSupported(string packageversion, string parameter)
        {
            throw new ArgumentException("The parameter ");
        }
    }
}
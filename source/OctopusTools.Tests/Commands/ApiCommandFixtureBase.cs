using System;
using System.Collections.Generic;
using log4net;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusTools.Commands;

namespace OctopusTools.Tests.Commands
{
    public abstract class ApiCommandFixtureBase
    {
        [SetUp]
        public void BaseSetup()
        {
            Log = Substitute.For<ILog>();

            RootResource rootDocument = Substitute.For<RootResource>();
            rootDocument.ApiVersion = "2.0";
            rootDocument.Version = "2.0";

            Repository = Substitute.For<IOctopusRepository>();
            Repository.Client.RootDocument.Returns(rootDocument);

            RepositoryFactory = Substitute.For<IOctopusRepositoryFactory>();
            RepositoryFactory.CreateRepository(null).ReturnsForAnyArgs(Repository);

            CommandLineArgs = new List<string>
            {
                "--server=http://the-server",
                "--apiKey=ABCDEF123456789"
            }; 
        }

        public ILog Log { get; set; }

        public IOctopusRepositoryFactory RepositoryFactory { get; set; }

        public IOctopusRepository Repository { get; set; }

        public List<string> CommandLineArgs { get; set; }

    }
}

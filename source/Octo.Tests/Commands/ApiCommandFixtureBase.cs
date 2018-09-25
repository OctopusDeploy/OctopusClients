using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octo.Tests.Commands
{
    public abstract class ApiCommandFixtureBase
    {
        private static string _previousCurrentDirectory;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            _previousCurrentDirectory = Directory.GetCurrentDirectory();
#if HAS_APP_CONTEXT
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
#else   
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
#endif
            
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Directory.SetCurrentDirectory(_previousCurrentDirectory);
        }

        [SetUp]
        public void BaseSetup()
        {
            LogOutput = new StringBuilder();

            Log = new LoggerConfiguration()
                .WriteTo.TextWriter(new StringWriter(LogOutput), outputTemplate: "{Message}{NewLine}{Exception}", formatProvider: new StringFormatter(null))
                .CreateLogger();
           
            RootResource rootDocument = Substitute.For<RootResource>();
            rootDocument.ApiVersion = "2.0";
            rootDocument.Version = "2.0";
            rootDocument.Links.Add("Tenants", "http://tenants.org");

            Repository = Substitute.For<IOctopusAsyncRepository>();
            Repository.Client.RootDocument.Returns(rootDocument);
            Repository.HasLink(Arg.Any<string>()).Returns(call => Repository.Client.RootDocument.HasLink(call.Arg<string>()));
            Repository.Link(Arg.Any<string>()).Returns(call => Repository.Client.RootDocument.Link(call.Arg<string>()));

            ClientFactory = Substitute.For<IOctopusClientFactory>();

            RepositoryFactory = Substitute.For<IOctopusAsyncRepositoryFactory>();
            RepositoryFactory.CreateRepository(null).ReturnsForAnyArgs(Repository);

            FileSystem = Substitute.For<IOctopusFileSystem>();

            CommandOutputProvider = new CommandOutputProvider(Log);

            CommandLineArgs = new List<string>
            {
                "--server=http://the-server",
                "--apiKey=ABCDEF123456789"
            };


        }

        public StringBuilder LogOutput { get; set; }
        public string[] LogLines => LogOutput.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        public IOctopusClientFactory ClientFactory { get; set; }

        public ILogger Log { get; set; }

        public ILogger FormattedOutputLogger { get; set; }

        public IOctopusAsyncRepositoryFactory RepositoryFactory { get; set; }

        public IOctopusAsyncRepository Repository { get; set; }

        public IOctopusFileSystem FileSystem { get; set; }

        public ICommandOutputProvider CommandOutputProvider { get; set; }

        public List<string> CommandLineArgs { get; set; }

        class StringFormatter : IFormatProvider
        {
            readonly IFormatProvider basedOn;
            public StringFormatter(IFormatProvider basedOn)
            {
                this.basedOn = basedOn;
            }

            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(string))
                {
                    return "s";
                }
                return this.basedOn;
            }
        }
    }
}

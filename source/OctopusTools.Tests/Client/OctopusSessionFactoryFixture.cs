using System;
using System.IO;
using System.Net;
using NSubstitute;
using NUnit.Framework;
using OctopusTools.Client;
using OctopusTools.Commands;
using log4net;
using OctopusTools.Infrastructure;
using OctopusTools.Model;

namespace OctopusTools.Tests.Client
{
    [TestFixture]
    class OctopusSessionFactoryFixture
    {
        string configFile;
        TestCommand command;
        IOctopusSessionFactory sessionFactory;
        IOctopusSession session;

        [SetUp]
        public void SetUp()
        {
            configFile = Path.GetTempFileName();
            sessionFactory = Substitute.For<IOctopusSessionFactory>();
            command = new TestCommand(sessionFactory, Substitute.For<ILog>());

            session = Substitute.For<IOctopusSession>();
            sessionFactory.OpenSession(Arg.Any<Uri>(), Arg.Any<NetworkCredential>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(session);
            session.RootDocument.Returns(new RootDocument());
        }

        [TearDown]
        public void TearDown()
        {
            if (configFile != null && File.Exists(configFile))
            {
                File.Delete(configFile);
            }
        }

        [Test]
        public void ShouldRequireServerUrlToBeSpecified()
        {
            var ex = Assert.Throws<CommandException>(() => command.Execute(new string[0]));

            Assert.That(ex.Message, Is.StringStarting("Please specify the Octopus Server URL"));
        }

        [Test]
        public void ShouldRequireApiKey()
        {
            var ex = Assert.Throws<CommandException>(() => command.Execute(new[] { "--server", "http://octopusServer" }));

            Assert.That(ex.Message, Is.StringStarting("Please specify your API key using"));
        }

        [Test]
        public void ShouldRequireValidServerUri()
        {
            var ex = Assert.Throws<CommandException>(() => command.Execute(new[] { "--server", "::vfduf", "--apiKey", "ABC" }));

            Assert.That(ex.Message, Is.StringStarting("Invalid URI format. Please specify an Octopus Server URL"));
        }

        [Test]
        public void ShouldRequireHttpOrHttpsServer()
        {
            var ex = Assert.Throws<CommandException>(() => command.Execute(new[] { "--server", "ftp://foo", "--apiKey", "ABC" }));

            Assert.That(ex.Message, Is.StringStarting("Invalid URI format, the URI should start with 'http' or 'https'"));
        }

        [Test]
        public void ShouldAllowUsernameAndPasswordToBeSpecified()
        {
            command.Execute(new[] { "--user", "domain\\fred", "--pass", "password", "--APIKEY", "ABCDE0123FG", "--server", "http://octopusServer" });

            sessionFactory.OpenSession(
                Arg.Any<Uri>(),
                Arg.Is<NetworkCredential>(cred => cred.UserName == "fred" && cred.Domain == "domain" && cred.Password == "password"),
                Arg.Any<string>(),
                Arg.Any<bool>());
        }

        [Test]
        public void ShouldAssignValuesFromConfigFileForValuesNotSpecifiedInCommandLineArgs()
        {
            File.WriteAllLines(configFile, new[]
            {
                "server=http://octopusServer",
                "apiKey=ABCDE0123FG",
                "user=domain\\fred",
                "password=password"
            });

            command.Execute(new[] { "--configFile", configFile });

            sessionFactory.OpenSession(
                Arg.Is<Uri>(u => u.ToString() == "http://octopusServer/"),
                Arg.Is<NetworkCredential>(cred => cred.UserName == "fred" && cred.Domain == "domain" && cred.Password == "password"),
                Arg.Is("ABCDE0123FG"),
                Arg.Any<bool>());
        }

        [Test]
        public void ShouldNotOverrideCommandLineArgWithValueFromConfigFile()
        {
            File.WriteAllLines(configFile, new[]
            {
                "server=http://octopusServer",
                "apiKey=ABCDE0123FG",
                "user=domain\\fred",
                "password=password"
            });

            command.Execute(new[] { "--configFile", configFile, "--apiKEY", "ABC" });

            sessionFactory.OpenSession(
                Arg.Is<Uri>(u => u.ToString() == "http://octopusServer/"),
                Arg.Is<NetworkCredential>(cred => cred.UserName == "fred" && cred.Domain == "domain" && cred.Password == "password"),
                Arg.Is("ABC"),
                Arg.Any<bool>());
        }

        [Test]
        public void ShouldThrowOnUnrecognizedArguments()
        {
            var ex = Assert.Throws<CommandException>(() => command.Execute(new[] {"--baz", "bam"}));

            Assert.That(ex.Message, Is.StringStarting("Unrecognized command arguments"));
        }

        class TestCommand : ApiCommand
        {
            public TestCommand(IOctopusSessionFactory client, ILog log)
                : base(client, log)
            {
            }

            protected override void Execute()
            {
            }
        }
    }
}

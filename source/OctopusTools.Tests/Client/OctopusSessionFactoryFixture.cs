using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using log4net;

namespace OctopusTools.Tests.Client
{
	[TestFixture]
	class OctopusSessionFactoryFixture
	{
		string configFile;

		[SetUp]
		public void SetUp()
		{
			configFile = Path.GetTempFileName();
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
		public void ShouldAssignValuesFromConfigFileForValuesNotSpecifiedInCommandLineArgs()
		{
			const string serverUrlDefinedInConfigFile = "http://octopusServer";
			const string apiKeyDefinedInConfigFile = "ABCDE0123FG";
			const string usernameDefinedInConfigFile = "username";
			const string passwordDefinedInConfigFile = "password";

			WriteConfigFile(serverUrlDefinedInConfigFile, apiKeyDefinedInConfigFile, usernameDefinedInConfigFile, passwordDefinedInConfigFile);

			var log = Substitute.For<ILog>();
			var commandLineArgsProvider = new CommandLineArgsProvider(new List<string> { "--configFile=" + configFile });
			var octopusSessionFactory = new TestableOctopusSessionFactory(log, commandLineArgsProvider);

			Assert.AreEqual(octopusSessionFactory.GetServerUrl(), serverUrlDefinedInConfigFile);
			Assert.AreEqual(octopusSessionFactory.GetApiKey(), apiKeyDefinedInConfigFile);
			Assert.AreEqual(octopusSessionFactory.GetUser(), usernameDefinedInConfigFile);
			Assert.AreEqual(octopusSessionFactory.GetPassword(), passwordDefinedInConfigFile);
		}

		[Test]
		public void ShouldNotOverrideCommandLineArgWithValueFromConfigFile()
		{
			const string passwordPassedInOnCommandLine = "password0";
			const string passwordDefinedInConfigFile = "password1";

			WriteConfigFile(null, null, null, passwordDefinedInConfigFile);

			var log = Substitute.For<ILog>();
			var commandLineArgsProvider = new CommandLineArgsProvider(new List<string> { "--pass=" + passwordPassedInOnCommandLine, "--configFile=" + configFile });
			var octopusSessionFactory = new TestableOctopusSessionFactory(log, commandLineArgsProvider);

			Assert.AreEqual(octopusSessionFactory.GetPassword(), passwordPassedInOnCommandLine);
		}

		private void WriteConfigFile(string serverUrl, string apiKey, string user, string pass)
		{
			var streamWriter = File.AppendText(configFile);

			if (serverUrl != null) streamWriter.WriteLine("server=" + serverUrl);
			if (apiKey != null) streamWriter.WriteLine("apiKey=" + apiKey);
			if (user != null) streamWriter.WriteLine("user=" + user);
			if (pass != null) streamWriter.WriteLine("pass=" + pass);

			streamWriter.Close();
		}
	}
	
	class TestableOctopusSessionFactory : OctopusSessionFactory
	{
		public TestableOctopusSessionFactory(ILog log, ICommandLineArgsProvider commandLineArgsProvider) : base(log, commandLineArgsProvider)
		{
		}

		public string GetServerUrl()
		{
			return serverBaseUrl;
		}
		public string GetApiKey()
		{
			return apiKey;
		}
		public string GetUser()
		{
			return user;
		}
		public string GetPassword()
		{
			return pass;
		}
	}
}

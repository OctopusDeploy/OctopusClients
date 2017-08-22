using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using Octopus.Cli.Tests.Commands;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Octopus.Cli.Commands.Tenant;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;

namespace Octo.Tests.Commands
{
    public class ListTenantsFixtures : ApiCommandFixtureBase
    {
        private ListTenantsCommand listTenantsCommand;

        [SetUp]
        public void Setup()
        {
            listTenantsCommand = new ListTenantsCommand(RepositoryFactory, Log, FileSystem, ClientFactory, CommandOutputProvider);
            Repository.Tenants.FindAll().Returns(Task.FromResult(new List<TenantResource>
            {
                new TenantResource {Name = "Tenant1", Id = "Tenant-1"},
                new TenantResource {Name = "Tenant2", Id = "Tenant-2"},
            }));

            Repository.FeaturesConfiguration.GetFeaturesConfiguration()
                .ReturnsForAnyArgs(new FeaturesConfigurationResource { IsMultiTenancyEnabled = true });
        }

        [Test]
        public async Task MultiTenacyFeatureDisabled_ShouldThrowException()
        {
            Repository.FeaturesConfiguration.GetFeaturesConfiguration()
                .ReturnsForAnyArgs(new FeaturesConfigurationResource {IsMultiTenancyEnabled = false});

            try
            {
                await listTenantsCommand.Execute(CommandLineArgs.ToArray());
                Assert.Fail("Should have thrown CommandException");

            }
            catch (CommandException commandException)
            {
                commandException.Message.Should().Contain("Multi-Tenancy");
            }
        }

        [Test]
        public async Task JsonFormat_ShouldBeWellFormed()
        {
            CommandLineArgs.Add("--outputFormat=json");
            await listTenantsCommand.Execute(CommandLineArgs.ToArray());
            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain("Tenant-1");
            logoutput.Should().Contain("Tenant-2");
        }
    }
}

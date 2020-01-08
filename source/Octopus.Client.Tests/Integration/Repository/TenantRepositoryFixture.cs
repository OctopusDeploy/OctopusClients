using System;
using System.IO;
using System.Reflection;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Integration.Repository
{
    public class TenantRepositoryTest : HttpIntegrationTestBase
    {
        public TenantRepositoryTest()
            : base(UrlPathPrefixBehaviour.UseNoPrefix) //as the canned responses have no prefix, it falls over if we try and isolate the tests with a different prefix
        {
            Get($"{TestRootPath}api", parameters =>
            {
                string content = GetCannedResponse(parameters, $"Octopus.Client.Tests.CannedResponses.2019.7.12.api.GET.json");
                return Response.AsText(content, "application/json");
            });
        }

        [Test]
        public void AsyncGetTasksReturnsAllPages()
        {
            var repository = new TenantRepository(new OctopusAsyncRepository(AsyncClient));
            Assert.ThrowsAsync<OperationNotSupportedByOctopusServerException>(
                async () => await repository.CreateOrModify("My Tenant", "Tenant Description"),
            "Tenant Descriptions requires Octopus version 2019.8.0 or newer.");
            Assert.ThrowsAsync<OperationNotSupportedByOctopusServerException>(
                async () => await repository.CreateOrModify("My Tenant", "Tenant Description", "Tenant-123"),
            "Cloning Tenants requires Octopus version 2019.8.0 or newer.");
        }

        [Test]
        public void SyncGetTasksReturnsAllPages()
        {
            var repository = new Client.Repositories.TenantRepository(SyncClient.Repository);
            Assert.Throws<OperationNotSupportedByOctopusServerException>(
                () => repository.CreateOrModify("My Tenant", "Tenant Description"),
                "Tenant Descriptions requires Octopus version 2019.8.0 or newer.");
            Assert.Throws<OperationNotSupportedByOctopusServerException>(
                () => repository.CreateOrModify("My Tenant", "Tenant Description", "Tenant-123"),
                "Cloning Tenants requires Octopus version 2019.8.0 or newer.");
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Integration.Repository
{
    public class UserRepositoryTest : HttpIntegrationTestBase
    {
        static int nextId = 100;

        // Dictionary must be static because the webserver spawned by HttpIntegrationTestBase considers
        // our test class to be an MVC-style controller, and creates a new instance for each HTTP request.
        // We need to refactor these things to move away from NancyModule
        static ConcurrentDictionary<string, UserResource> users = new ConcurrentDictionary<string, UserResource>();

        [SetUp]
        public void SetUp() => users.Clear();

        public UserRepositoryTest() : base(UrlPathPrefixBehaviour.UseNoPrefix)
        {
            Get($"{TestRootPath}api/users/{{id}}", parameters =>
            {
                string userId = parameters.id;
                if (users.TryGetValue(userId, out var user)) return user;

                Context.Response.StatusCode = HttpStatusCode.NotFound;
                return null!;
            });

            Post($"{TestRootPath}api/users", parameters =>
            {
                var requestAsString = Context.Request.Body.AsString();
                var user = JsonConvert.DeserializeObject<UserResource>(requestAsString,
                    JsonSerialization.GetDefaultSerializerSettings());

                // pretend we saved it
                user.Id = $"Users-{Interlocked.Increment(ref nextId)}";
                users.TryAdd(user.Id, user);
                user.Links.Add("self", $"{TestRootPath}api/users/{user.Id}");

                return user;
            });
        }

        [Test]
        public async Task CreateWithUserName_CreatesActiveUser()
        {
            var repository = new UserRepository(new OctopusAsyncRepository(AsyncClient));

            var userResource = await repository.Create("new-username", "new-displayname");

            userResource.IsActive.Should().BeTrue();
        }

        [Test]
        public async Task CreateByResource_CreatesActiveUserByDefault()
        {
            var repository = new UserRepository(new OctopusAsyncRepository(AsyncClient));

            var inputResource = new UserResource
            {
                Username = "new-username",
                DisplayName = "new-displayname",
                // deliberately not setting IsActive
            };
            var userResource = await repository.Create(inputResource);

            userResource.IsActive.Should().BeTrue();
        }

        [Test]
        public async Task CreateByResource_CanCreateActiveUser()
        {
            var repository = new UserRepository(new OctopusAsyncRepository(AsyncClient));

            var inputResource = new UserResource
            {
                Username = "new-username",
                DisplayName = "new-displayname",
                IsActive = true
            };
            var userResource = await repository.Create(inputResource);

            userResource.IsActive.Should().BeTrue();
        }

        [Test]
        public async Task CreateByResource_CanCreateInactiveUser()
        {
            var repository = new UserRepository(new OctopusAsyncRepository(AsyncClient));

            var inputResource = new UserResource
            {
                Username = "new-username",
                DisplayName = "new-displayname",
                IsActive = false
            };
            var userResource = await repository.Create(inputResource);

            userResource.IsActive.Should().BeFalse();
        }

        [Test]
        public async Task CreateServiceAccount_CreatesActiveUser()
        {
            var repository = new UserRepository(new OctopusAsyncRepository(AsyncClient));

            var userResource = await repository.CreateServiceAccount("new-username", "new-displayname");

            userResource.IsActive.Should().BeTrue();
        }
    }
}

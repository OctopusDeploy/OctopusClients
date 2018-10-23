#if SYNC_CLIENT
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensions;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories
{
    public class OctopusRepositoryTests
    {
        private readonly string[] delayInitialisedProperties = { nameof(OctopusAsyncRepository.SpaceContext) };
        [Test]
        public void AllPropertiesAreNotNullForDefaultSpaceRepository()
        {
            var client = Substitute.For<IOctopusClient>();
            client.Get<UserResource>(Arg.Any<string>()).Returns(new UserResource() { Links = { { "Spaces", "" } } });
            client.Get<SpaceResource[]>(Arg.Any<string>()).Returns(new[] { new SpaceResource() { Id = "Spaces-1", IsDefault = true} });
            client.Get<SpaceRootResource>(Arg.Any<string>(), Arg.Any<object>()).Returns(new SpaceRootResource());
            client.Get<RootResource>(Arg.Any<string>()).Returns(new RootResource()
            {
                ApiVersion = "3.0.0",
                Links =
                {
                    {"CurrentUser",  ""},
                    {"SpaceHome",  ""},
                }
            });
            Assertion(client);
        }

        [Test]
        public void AllPropertiesAreNotNullForSystemOnlyRepository()
        {
            var client = Substitute.For<IOctopusClient>();
            client.Get<RootResource>(Arg.Any<string>()).Returns(new RootResource()
            {
                ApiVersion = "3.0.0",
                Links = LinkCollection.Self("/api")
                    .Add("CurrentUser", "/api/users/me")
            });
            client.Get<UserResource>(Arg.Any<string>()).Throws(new OctopusSecurityException(401, "Test"));
            Assertion(client);
        }

        private void Assertion(IOctopusClient client)
        {
            var repository = new OctopusRepository(client);
            var nullPropertiesQ = from p in typeof(OctopusRepository).GetProperties()
                where !delayInitialisedProperties.Contains(p.Name)
                where p.GetMethod.Invoke(repository, new object[0]) == null
                select p.Name;

            var nullProperties = nullPropertiesQ.ToArray();
            if (nullProperties.Any())
                Assert.Fail("The following properties are null after OctopusAsyncRepository instantiation: " + nullProperties.CommaSeperate());
        }
    }
}
#endif
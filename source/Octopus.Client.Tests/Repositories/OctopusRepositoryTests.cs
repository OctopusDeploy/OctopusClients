#if SYNC_CLIENT
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Octopus.Client.Extensions;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories
{
    public class OctopusRepositoryTests
    {
        [Test]
        public void AllPropertiesAreNotNullForDefaultSpaceRepository()
        {
            var client = Substitute.For<IOctopusClient>();
            client.IsAuthenticated.Returns(true);
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
            var repository = new OctopusRepository(client);
            var nullPropertiesQ = from p in typeof(OctopusRepository).GetProperties()
                where p.GetMethod.Invoke(repository, new object[0]) == null
                select p.Name;

            var nullProperties = nullPropertiesQ.ToArray();
            if (nullProperties.Any())
                Assert.Fail("The following properties are null after OctopusAsyncRepository instantiation: " + nullProperties.CommaSeperate());
        }

        [Test]
        public void SpaceRootDocumentIsNullForSystemOnlyRepository()
        {
            var client = Substitute.For<IOctopusClient>();
            client.IsAuthenticated.Returns(false);
            client.Get<RootResource>(Arg.Any<string>()).Returns(new RootResource() { ApiVersion = "3.0.0" });
            var repository = new OctopusRepository(client);
            var nullPropertiesQ = from p in typeof(OctopusRepository).GetProperties()
                where p.GetMethod.Invoke(repository, new object[0]) == null
                select p.Name;

            var nullProperties = nullPropertiesQ.ToArray();
            nullProperties.Single().Should().Be("SpaceRootDocument");
        }
    }
}
#endif
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Octopus.Client.Extensions;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories
{
    public class OctopusAsyncRepositoryTests
    {
        [Test]
        public void AllPropertiesAreNotNullForASpaceRepository()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            client.IsAuthenticated.Returns(true);
            client.Get<UserResource>(Arg.Any<string>()).Returns(Task.FromResult(new UserResource() { Links = {{ "Spaces", "" } }}));
            client.Get<SpaceResource[]>(Arg.Any<string>()).Returns(Task.FromResult(new[] {new SpaceResource() {Id = "Spaces-1"}}));
            client.Get<SpaceRootResource>(Arg.Any<string>(), Arg.Any<object>()).Returns(Task.FromResult(new SpaceRootResource()));
            client.Get<RootResource>(Arg.Any<string>()).Returns(new RootResource()
            {
                ApiVersion = "3.0.0",
                Links =
                {
                    {"CurrentUser",  ""},
                    {"SpaceHome",  ""},
                }
            });
            var repository = OctopusAsyncRepository.Create(client, SpaceContext.SpecificSpaceAndSystem("Spaces-1")).Result;
            var nullPropertiesQ = from p in typeof(OctopusAsyncRepository).GetTypeInfo().GetProperties()
                where p.GetMethod.Invoke(repository, new object[0]) == null
                select p.Name;

            var nullProperties = nullPropertiesQ.ToArray();
            if (nullProperties.Any())
                Assert.Fail("The following properties are null after OctopusAsyncRepository instantiation: " + nullProperties.CommaSeperate());
        }

        [Test]
        public void SpaceRootDocumentPropertyIsNullForSystemOnlyRepository()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            client.IsAuthenticated.Returns(false);
            client.Get<RootResource>(Arg.Any<string>()).Returns(new RootResource() { ApiVersion = "3.0.0" });
            var repository = OctopusAsyncRepository.Create(client, SpaceContext.SystemOnly()).Result;
            var nullPropertiesQ = from p in typeof(OctopusAsyncRepository).GetTypeInfo().GetProperties()
                where p.GetMethod.Invoke(repository, new object[0]) == null
                select p.Name;

            var nullProperties = nullPropertiesQ.ToArray();
            nullProperties.Single().Should().Be("SpaceRootDocument");
        }
    }
}
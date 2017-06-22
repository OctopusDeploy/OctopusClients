using NUnit.Framework;
using System.Linq;
using System.Reflection;
using NSubstitute;
using Octopus.Client.Extensions;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories
{
    public class OctopusAsyncRepositoryTests
    {
        [Test]
        public void AllPropertiesAreNotNull()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            client.RootDocument.Returns(new RootResource());
            var repository = new OctopusAsyncRepository(client);
            var nullPropertiesQ = from p in typeof(OctopusAsyncRepository).GetTypeInfo().GetProperties()
                where p.GetMethod.Invoke(repository, new object[0]) == null
                select p.Name;

            var nullProperties = nullPropertiesQ.ToArray();
            if (nullProperties.Any())
                Assert.Fail("The following properties are null after OctopusAsyncRepository instantiation: " + nullProperties.CommaSeperate());
        }
    }
}
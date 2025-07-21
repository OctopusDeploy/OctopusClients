﻿using NUnit.Framework;
using System.Linq;
using System.Reflection;
using NSubstitute;
using Octopus.Client.Extensions;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories
{
    public class OctopusAsyncRepositoryTests
    {
        private readonly string[] delayInitialisedProperties = { nameof(OctopusAsyncRepository.Scope) };
        [Test]
        public void AllPropertiesAreNotNullExceptDelayInitialised()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            client.Repository.LoadRootDocument().Returns(new RootResource
            {
                Version = "2099.0.0"
            });
            var repository = new OctopusAsyncRepository(client);
            var nullPropertiesQ = from p in typeof(OctopusAsyncRepository).GetTypeInfo().GetProperties()
                where !delayInitialisedProperties.Contains(p.Name)
                                  where p.GetMethod.Invoke(repository, new object[0]) == null
                select p.Name;

            var nullProperties = nullPropertiesQ.ToArray();
            if (nullProperties.Any())
                Assert.Fail("The following properties are null after OctopusAsyncRepository instantiation: " + nullProperties.CommaSeparate());
        }
    }
}
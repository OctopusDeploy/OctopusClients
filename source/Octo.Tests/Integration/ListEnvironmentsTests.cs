using System;
using System.Collections.Generic;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Integration
{
    public class ListEnvironmentsTests : IntegrationTestBase
    {
        private const string EnvironmentName = "Foo Environment";

        public ListEnvironmentsTests()
        {
            Get($"{TestRootPath}/api/environments", p => Response.AsJson(
                new ResourceCollection<EnvironmentResource>(
                    new[] {
                        new EnvironmentResource
                        {
                            Name = EnvironmentName
                        }
                    },
                    new LinkCollection()
                )
            ));
        }

        [Test]
        public void ListEnvironments()
        {
            var result = Execute("list-environments");
            result.LogOutput.Should().Contain(EnvironmentName);
            result.Code.Should().Be(0);
        }

    }
}
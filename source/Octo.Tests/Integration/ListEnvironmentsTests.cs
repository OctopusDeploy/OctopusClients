using System;
using System.Collections.Generic;

using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Integration
{
    public class ListEnvironmentsTests : IntegrationTestBase
    {
        private const string EnvironmentName = "Foo Environment";

        public ListEnvironmentsTests()
        {
            Get($"{TestRootPath}/api/users/me", p => Response.AsJson(
                new UserResource()
                {
                    Links = new LinkCollection()
                    {
                        {"Spaces", TestRootPath + "/api/users/users-1/spaces" }
                    }
                }
            ));

            Get($"{TestRootPath}/api/users/users-1/spaces", p => Response.AsJson(
                    new[] {
                        new SpaceResource() { Id = "Spaces-1", IsDefault = true},
                        new SpaceResource() { Id = "Spaces-2", IsDefault = false}
                    }
            ));

            Get($"{TestRootPath}/api/spaces-1", p => Response.AsJson(
                new SpaceRootResource()
            ));

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
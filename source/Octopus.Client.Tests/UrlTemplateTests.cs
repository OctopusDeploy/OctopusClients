using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.Tests
{
    [TestFixture]
    public class UrlTemplateTests
    {
        [Test]
        public void NoParameters()
        {
            // arrange
            var urlTemplate = @"/api/serverstatus/timezones";
            var urlTemplateResolver = new UrlTemplate(urlTemplate);

            // act
            var url = urlTemplateResolver.Resolve();

            // assert
            url.Should().BeEquivalentTo(urlTemplate);
        }

        [Test]
        public void SingleQueryParameter()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate(@"/api/users/login{?returnUrl}");
            urlTemplateResolver.SetParameter("returnUrl", "123");

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/users/login?returnUrl=123");
        }

        [Test]
        public void SingleQueryParameterNoValueProvided()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate(@"/api/users/login{?returnUrl}");

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/users/login");
        }

        [Test]
        public void SingleUrlParameter()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate(@"/api/projecttriggers{/id}");
            urlTemplateResolver.SetParameter("id", 1);

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/projecttriggers/1");
        }

        [Test]
        public void MultipleOptionalQueryParameters_SingleValueProvided()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate(@"/api/channels/rule-test{?version,versionRange,preReleaseTag}");
            urlTemplateResolver.SetParameter("versionRange", "1.2");

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/channels/rule-test?versionRange=1.2");
        }

        [Test]
        public void MultipleOptionalQueryParameters_AllValuesProvided()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate(@"/api/channels/rule-test{?version,versionRange,preReleaseTag}");
            urlTemplateResolver.SetParameter("versionRange", "1.2");
            urlTemplateResolver.SetParameter("version", "2.0.0");
            urlTemplateResolver.SetParameter("preReleaseTag", "tag");

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/channels/rule-test?version=2.0.0&versionRange=1.2&preReleaseTag=tag");
        }

        [Test]
        public void OptionalUrlAndQueryParameter_UrlValueNotProvided()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate("/api/subscriptions{/id}{?skip,take,ids}");
            urlTemplateResolver.SetParameter("skip", 10);
            
            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/subscriptions?skip=10");
        }

        [Test]
        public void OptionalUrlAndQueryParameter_UrlAndQueryValueProvided()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate("/api/subscriptions{/id}{?skip,take,ids}");
            urlTemplateResolver.SetParameter("skip", 10);
            urlTemplateResolver.SetParameter("id", 6);

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/subscriptions/6?skip=10");
        }

        [Test]
        public void ParameterProvidedThatIsNotInTemplate()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate("/api/subscriptions{/id}{?skip,take,ids}");
            urlTemplateResolver.SetParameter("skiptake", 10);
            urlTemplateResolver.SetParameter("id", 6);

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/subscriptions/6");
        }

        [Test]
        public void ParameterValueContainsNonAsciiCharacter_ShouldUrlEncodeValue()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate(@"/api/projects{/id}{?name,skip,ids,clone,take}");
            urlTemplateResolver.SetParameter("name", "KPP.Bastjänster");
            
            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/projects?name=KPP.Bastj%C3%A4nster");
        }

        [Test]
        public void ParameterValueContainsSmilyFace_ShouldUrlEncodeValue()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate(@"/api/tenants{/id}{?skip,projectId,name,tags,take,ids,partialName}");
            urlTemplateResolver.SetParameter("name", @"Team 😄");

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/tenants?name=Team%20%F0%9F%98%84");
        }

        [Test]
        public void ParameterValueContainsReservedCharacter_ShouldUrlEncodeValue()
        {
            // Arrange
            var urlTemplateResolver = new UrlTemplate(@"/api/projects{/id}{?name,skip,ids,clone,take}");
            urlTemplateResolver.SetParameter("name", "Me&You");

            // Act
            var url = urlTemplateResolver.Resolve();

            // Assert
            url.Should().BeEquivalentTo(@"/api/projects?name=Me%26You");
        }
    }
}
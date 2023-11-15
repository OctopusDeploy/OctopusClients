using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.HttpRouting;
using Octopus.Server.MessageContracts.Base;
using Octopus.Server.MessageContracts.Base.Attributes;
using Octopus.Server.MessageContracts.Base.HttpRoutes;

namespace Octopus.Client.Tests.HttpRequestRouting
{
    public class GivenAGetPayload
    {
        [Test]
        [TestCaseSource(nameof(ValidTestCases))]
        public void ValidRoutesShouldResolve(SomeRequest command, string expected)
        {
            var httpRouteExtractor = new HttpRouteExtractor(AppDomainScanner.ScanForAllTypes);

            var httpRoute = httpRouteExtractor.ExtractHttpRoute(command);

            httpRoute.Should().Be(expected);
        }

        [Test]
        [TestCaseSource(nameof(InvalidTestCases))]
        public void InvalidRoutesShouldThrow(SomeRequest command)
        {
            var httpRouteExtractor = new HttpRouteExtractor(AppDomainScanner.ScanForAllTypes);

            Action action = () => httpRouteExtractor.ExtractHttpRoute(command);

            action.Should().Throw<PayloadRoutingException>();
        }

        public static IEnumerable<TestCaseData> ValidTestCases()
        {
            yield return new TestCaseData(new SomeRequest {Foo = "foo"}, "/api/some-request/foo");
            yield return new TestCaseData(new SomeRequest {Foo = "!@#$%^&*()+/\\"}, "/api/some-request/!%40%23%24%25%5E%26*()%2B%2F%5C");
            yield return new TestCaseData(new SomeRequest {Foo = "foo", Baz = false},
                "/api/some-request/foo?baz=False");
            yield return new TestCaseData(new SomeRequest {Foo = "foo", Bar = 42}, "/api/some-request/foo?bar=42");
            yield return new TestCaseData(new SomeRequest {Foo = "foo", Bar = 13, Baz = false},
                "/api/some-request/foo?bar=13&baz=False");
        }

        public static IEnumerable<TestCaseData> InvalidTestCases()
        {
            yield return new TestCaseData(new SomeRequest());
            yield return new TestCaseData(new SomeRequest {Bar = 42});
            yield return new TestCaseData(new SomeRequest {Bar = 13, Baz = false});
            yield return new TestCaseData(new SomeRequest {Baz = false});
        }

        [Test]
        public void TheHttpMethodShouldBeCorrect()
        {
            var httpRouteExtractor = new HttpRouteExtractor(AppDomainScanner.ScanForAllTypes);

            var request = new SomeRequest();
            var httpMethod = httpRouteExtractor.ExtractHttpMethod(request);

            httpMethod.Should().Be(System.Net.Http.HttpMethod.Get);
        }


        public class SomeRequest : IRequest<SomeRequest, SomeResponse>
        {
            [Required] public string Foo { get; set; }

            [Optional] public int? Bar { get; set; }

            [Optional] public bool? Baz { get; set; }

            public override string ToString() => $"{Foo}, {Bar}, {Baz}";
        }

        public class SomeResponse : IResponse
        {
        }

        public class SomeRequestHttpRoute : IHttpRequestRouteFor<SomeRequest, SomeResponse>
        {
            [HttpRouteTemplate] public const string RouteTemplate = "/api/some-request/{Foo}";

            public HttpMethod HttpMethod { get; } = HttpMethod.Get;
        }
    }
}
using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.HttpRouting;
using Octopus.Server.MessageContracts.Base;
using Octopus.Server.MessageContracts.Base.HttpRoutes;

namespace Octopus.Client.Tests.HttpRequestRouting
{
    public class HttpRouteExtractionFixture
    {
        [Test]
        public void TheRouteTemplateShouldBeCorrect()
        {
            var httpRouteExtractor = new HttpRouteExtractor(AppDomainScanner.ScanForAllTypes);

            var request = new SomeRequest();
            var route = httpRouteExtractor.ExtractHttpRoute(request);

            route.Should().Be(SomeRequestHttpRoute.RouteTemplate);
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
        }

        public class SomeResponse : IResponse
        {
        }

        public class SomeRequestHttpRoute : IHttpRequestRouteFor<SomeRequest, SomeResponse>
        {
            [HttpRouteTemplate] public const string RouteTemplate = "/api/some-request";

            public HttpMethod HttpMethod { get; } = HttpMethod.Get;
        }
    }
}
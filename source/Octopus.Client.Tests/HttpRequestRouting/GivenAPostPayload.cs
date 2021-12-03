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
    public class GivenAPostPayload
    {
        [Test]
        [TestCaseSource(nameof(ValidTestCases))]
        public void ValidRoutesShouldResolve(SomeCommand command, string expected)
        {
            var httpRouteExtractor = new HttpRouteExtractor(AppDomainScanner.ScanForAllTypes);

            var httpRoute = httpRouteExtractor.ExtractHttpRoute(command);

            httpRoute.Should().Be(expected);
        }

        [Test]
        [TestCaseSource(nameof(InvalidTestCases))]
        public void InvalidRoutesShouldThrow(SomeCommand command)
        {
            var httpRouteExtractor = new HttpRouteExtractor(AppDomainScanner.ScanForAllTypes);

            Action action = () => httpRouteExtractor.ExtractHttpRoute(command);

            action.ShouldThrow<PayloadRoutingException>();
        }

        public static IEnumerable<TestCaseData> ValidTestCases()
        {
            yield return new TestCaseData(new SomeCommand {Foo = "foo"}, "/api/some-command/foo");
            yield return new TestCaseData(new SomeCommand {Foo = "foo", Baz = false}, "/api/some-command/foo");
            yield return new TestCaseData(new SomeCommand {Foo = "foo", Bar = 42}, "/api/some-command/foo/42");
            yield return new TestCaseData(new SomeCommand {Foo = "foo", Bar = 13, Baz = false},
                "/api/some-command/foo/13/False");
        }

        public static IEnumerable<TestCaseData> InvalidTestCases()
        {
            yield return new TestCaseData(new SomeCommand());
            yield return new TestCaseData(new SomeCommand {Bar = 42});
            yield return new TestCaseData(new SomeCommand {Bar = 13, Baz = false});
            yield return new TestCaseData(new SomeCommand {Baz = false});
        }

        public class SomeCommand : ICommand<SomeCommand, SomeResponse>
        {
            [Required] public string Foo { get; set; }

            [Optional] public int? Bar { get; set; }

            [Optional] public bool? Baz { get; set; }

            public override string ToString() => $"{Foo}, {Bar}, {Baz}";
        }

        public class SomeCommandHttpRoute : IHttpCommandRouteFor<SomeCommand, SomeResponse>
        {
            [HttpRouteTemplate] public const string RouteTemplateWithFoo = "/api/some-command/{foo}";
            [HttpRouteTemplate] public const string RouteTemplateWithFooBar = "/api/some-command/{foo}/{bar}";
            [HttpRouteTemplate] public const string RouteTemplateWithFooBarBaz = "/api/some-command/{foo}/{bar}/{baz}";
            public HttpMethod HttpMethod { get; } = HttpMethod.Post;
        }

        public class SomeResponse : IResponse
        {
        }
    }
}
using System;
using FluentAssertions;
using Nancy;
using Nancy.ModelBinding;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class HttpMethodTests : OctopusClientTestBase
    {
        private static string _lastMethod;

        public HttpMethodTests()
        {
            Get["/"] = p => Response.AsJson(new TestDto() {Value = "42"});

            Post["/"] = p =>
            {
                var dto = this.Bind<TestDto>();
                if (dto.Value != "Foo")
                    return CreateErrorResponse($"Value is not 'Foo', found '{dto.Value}'");

                _lastMethod = "Post";
                return HttpStatusCode.NoContent;
            };

            Put["/"] = p =>
            {
                var dto = this.Bind<TestDto>();
                if (dto.Value != "Foo")
                    return CreateErrorResponse($"Value is not 'Foo', found '{dto.Value}'");

                _lastMethod = "Put";
                return HttpStatusCode.NoContent;
            };

            Delete["/"] = p => _lastMethod = "Delete";
        }

        [Test]
        public void GetReturnsAValue()
        {
            Client.Get<TestDto>("/").Value.Should().Be("42");
        }

        [Test]
        public void PostingAObjectWorks()
        {
            _lastMethod = null;
            Action post = () => Client.Post("/", new TestDto {Value = "Foo"});
            post.ShouldNotThrow();
            _lastMethod.Should().Be("Post");
        }

        [Test]
        public void PuttingAObjectWorks()
        {
            _lastMethod = null;
            Action put = () => Client.Put("/", new TestDto {Value = "Foo"});
            put.ShouldNotThrow();
            _lastMethod.Should().Be("Put");
        }

        [Test]
        public void DeleteReachesTheServer()
        {
            _lastMethod = null;
            Action delete = () => Client.Delete("/");
            delete.ShouldNotThrow();
            _lastMethod.Should().Be("Delete");
        }

        class TestDto
        {
            public string Value { get; set; }
        }
    }
}
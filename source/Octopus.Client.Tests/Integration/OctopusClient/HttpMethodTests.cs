using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using Nancy.ModelBinding;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class HttpMethodTests : HttpIntegrationTestBase
    {
        private static string _lastMethod;

        public HttpMethodTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get(TestRootPath, p => Response.AsJson(new TestDto() { Value = "42" }));

            Post(TestRootPath, p =>
            {
                var dto = this.Bind<TestDto>();
                if (dto.Value != "Foo")
                    return CreateErrorResponse($"Value is not 'Foo', found '{dto.Value}'");

                _lastMethod = "Post";
                return HttpStatusCode.NoContent;
            });

            Put(TestRootPath, p =>
            {
                var dto = this.Bind<TestDto>();
                if (dto.Value != "Foo")
                    return CreateErrorResponse($"Value is not 'Foo', found '{dto.Value}'");

                _lastMethod = "Put";
                return HttpStatusCode.NoContent;
            });

            Delete(TestRootPath, p => _lastMethod = "Delete");
            
        }

        [Test]
        public void GetReturnsAValue()
        {
            var dto = AsyncClient.Get<TestDto>("~/").Result;
            dto.Value.Should().Be("42");
        }

        [Test]
        public async Task PostingAObjectWorks()
        {
            _lastMethod = null;
            Func<Task> post = () => AsyncClient.Post("~/", new TestDto { Value = "Foo" });
            await post.Should().NotThrowAsync();
            _lastMethod.Should().Be("Post");
        }

        [Test]
        public async Task PuttingAObjectWorks()
        {
            _lastMethod = null;
            Func<Task> put = () => AsyncClient.Put("~/", new TestDto { Value = "Foo" });
            await put.Should().NotThrowAsync();
            _lastMethod.Should().Be("Put");
        }

        [Test]
        public async Task DeleteReachesTheServer()
        {
            _lastMethod = null;
            Func<Task> delete = () => AsyncClient.Delete("~/");
            await delete.Should().NotThrowAsync();
            _lastMethod.Should().Be("Delete");
        }

        class TestDto
        {
            public string Value { get; set; }
        }
    }
}
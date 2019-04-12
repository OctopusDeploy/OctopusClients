using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Repositories.Async
{
    [TestFixture]
    public class BasicRepositoryFixture
    {
        private TestSpaceResourceAsyncRepository subject;
        private IOctopusAsyncRepository mockRepo;
        private SpaceResource defaultSpace;

        [SetUp]
        public void Setup()
        {
            mockRepo = Substitute.For<IOctopusAsyncRepository>();
            
            subject = new TestSpaceResourceAsyncRepository(mockRepo, "", async repo => await Task.FromResult(""));
            
            defaultSpace = new SpaceResource
            {
                Id = "Spaces-1",
                Name = "Default Space",
                IsDefault = true
            };
            mockRepo.Scope.Returns(RepositoryScope.ForSpace(defaultSpace));
        }
        
        [Test]
        public void Create_ResourceWithImpliedSpace_DefaultRepoScopeSetsTheSpace()
        {
            var resource = CreateProjectResourceForSpace(null);
            Assert.DoesNotThrow(() => subject.Create(resource));

            resource.SpaceId.Should()
                .Be(defaultSpace.Id, $"the repository scope will be used to enrich the spaceResource's {nameof(IHaveSpaceResource.SpaceId)} property");
        }
        
        [Test]
        public void Create_ResourceWithImpliedSpace_RepoScopeSetsTheSpace()
        {
            SetupRepositoryToUseADifferentSpace();
            var resource = CreateProjectResourceForSpace(null);
            Assert.DoesNotThrow(() => subject.Create(resource));

            resource.SpaceId.Should()
                .Be("Spaces-2", $"the repository scope will be used to enrich the spaceResource's {nameof(IHaveSpaceResource.SpaceId)} property");
        }

        [Test]
        public void Create_ResourceInExplicitSpace_NonMatchingSpaceRepoShouldOverwrite()
        {
            var resource = CreateProjectResourceForSpace("Spaces-2");
            Assert.DoesNotThrow(() => subject.Create(resource).Wait());

            resource.SpaceId.Should().Be(defaultSpace.Id, because: "https://github.com/OctopusDeploy/OctopusDeploy/blob/ff86277e425b2f2f7e5093a01cc7bc948bfbfca3/source/Octopus.IntegrationTests/Octopus.Server/Spaces/MixedScopeTest.cs#L127");
        }


        [Test]
        public void Create_ResourceInExplicitSpace_MatchingSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(defaultSpace.Id);
            Assert.DoesNotThrow(() => subject.Create(resource).Wait());
            resource.SpaceId.Should()
                .Be(defaultSpace.Id, $"the space resource {nameof(IHaveSpaceResource.SpaceId)} shouldn't have changed");
        }
        
        [Test]
        public void Update_ResourceInExplicitSpace_MatchingSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(defaultSpace.Id);
            
            Assert.DoesNotThrow(() => subject.Modify(resource).Wait());
            resource.SpaceId.Should()
                .Be(defaultSpace.Id, $"the space resource {nameof(IHaveSpaceResource.SpaceId)} shouldn't have changed");
        }
        
        [Test]
        public void Update_ResourceInExplicitSpace_NonMatchingSpaceRepoShouldThrow()
        {
            var resource = CreateProjectResourceForSpace("Spaces-2");
            Action activityUnderTest = () => subject.Modify(resource).Wait();
            
            activityUnderTest
                .ShouldThrow<ArgumentException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }

        [Test]
        public void Update_ResourceWithImpliedSpace_DefaultSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(null);
            Assert.DoesNotThrow(() => subject.Modify(resource).Wait());
            
        }
        
        [Test]
        public void Update_ResourceWithImpliedSpace_AnyOtherSpaceShouldThrow()
        {
            SetupRepositoryToUseADifferentSpace();
            var resource = CreateProjectResourceForSpace(null);
            Action activityUnderTest = () => subject.Modify(resource).Wait();
            
            activityUnderTest
                .ShouldThrow<ArgumentException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-2, or use a repository that is scoped to the default space.");
        }

        [Test]
        public void Delete_ResourceInExplicitSpace_NonMatchingSpaceRepoShouldThrow()
        {
            var resource = CreateProjectResourceForSpace("Spaces-2");
            Action activityUnderTest = () => subject.Delete(resource).Wait();
            
            activityUnderTest
                .ShouldThrow<ArgumentException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }
        
        [Test]
        public void Delete_ResourceInExplicitSpace_MatchingSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(defaultSpace.Id);
            Assert.DoesNotThrow(() => subject.Delete(resource).Wait());
        }
        
        [Test]
        public void Delete_ResourceWithImpliedSpace_DefaultSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(null);
            Assert.DoesNotThrow(() => subject.Delete(resource).Wait());

        }
        
        [Test]
        public void Delete_ResourceWithImpliedSpace_AnyOtherSpaceRepoShouldThrow()
        {
            SetupRepositoryToUseADifferentSpace();
            var resource = CreateProjectResourceForSpace(null);
            Action activityUnderTest = () => subject.Delete(resource).Wait();

            activityUnderTest
                .ShouldThrow<ArgumentException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-2, or use a repository that is scoped to the default space.");
        }

        private void SetupRepositoryToUseADifferentSpace()
        {
            mockRepo.Scope.Returns(RepositoryScope.ForSpace(new SpaceResource {Id = "Spaces-2", IsDefault = false}));
        }

        private ProjectResource CreateProjectResourceForSpace(string spaceId)
        {
            return new ProjectResource
            {
                Name = "Foo", 
                SpaceId = spaceId, 
                Links = new LinkCollection { {"Self", ""} }
            };
        }

        private class TestSpaceResourceAsyncRepository : BasicRepository<ProjectResource>
        {
            public TestSpaceResourceAsyncRepository(IOctopusAsyncRepository repository, string collectionLinkName, Func<IOctopusAsyncRepository, Task<string>> getCollectionLinkName = null) : base(repository, collectionLinkName, getCollectionLinkName)
            {
            }
        }
    }
}
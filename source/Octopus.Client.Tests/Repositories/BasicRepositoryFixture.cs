using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Tests.Repositories
{
    [TestFixture]
    public class BasicRepositoryFixture
    {
        private TestSpaceResourceRepository subject;
        private IOctopusRepository mockRepo;
        private SpaceResource defaultSpace;

        [SetUp]
        public void Setup()
        {
            mockRepo = Substitute.For<IOctopusRepository>();
            
            subject = new TestSpaceResourceRepository(mockRepo, "", repo => "");
            
            defaultSpace = new SpaceResource
            {
                Id = "Spaces-1",
                Name = "Default Space",
                IsDefault = true
            };
            mockRepo.Scope.Returns(RepositoryScope.ForSpace(defaultSpace));
        }

        [Test]
        public void Create_ResourceWithNoSpaceId_DefaultRepoScopeSetsTheSpace()
        {
            var resource = CreateProjectResourceForSpace(null);
            Assert.DoesNotThrow(() => subject.Create(resource));

            resource.SpaceId.Should()
                .Be(defaultSpace.Id, $"the repository scope will be used to enrich the spaceResource's {nameof(IHaveSpaceResource.SpaceId)} property");
        }
        
        [Test]
        public void Create_ResourceWithNoSpaceId_RepoScopeSetsTheSpace()
        {
            SetupRepositoryToUseADifferentSpace();
            var resource = CreateProjectResourceForSpace(null);
            Assert.DoesNotThrow(() => subject.Create(resource));

            resource.SpaceId.Should()
                .Be("Spaces-2", $"the repository scope will be used to enrich the spaceResource's {nameof(IHaveSpaceResource.SpaceId)} property");
        }

        [Test]
        public void Create_ResourceInExplicitSpace_NonMatchingSpaceRepoShouldThrow()
        {
            var resource = CreateProjectResourceForSpace("Spaces-2");
            Action activityUnderTest = () => subject.Create(resource);
            
            activityUnderTest
                .ShouldThrow<ArgumentException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }


        [Test]
        public void Create_ResourceInExplicitSpace_MatchingSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(defaultSpace.Id);
            Assert.DoesNotThrow(() => subject.Create(resource));
            resource.SpaceId.Should()
                .Be(defaultSpace.Id, $"the space resource {nameof(IHaveSpaceResource.SpaceId)} shouldn't have changed");
        }
        
        [Test]
        public void Update_ResourceInExplicitSpace_MatchingSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(defaultSpace.Id);
            
            Assert.DoesNotThrow(() => subject.Modify(resource));
            resource.SpaceId.Should()
                .Be(defaultSpace.Id, $"the space resource {nameof(IHaveSpaceResource.SpaceId)} shouldn't have changed");
        }
        
        [Test]
        public void Update_ResourceInExplicitSpace_NonMatchingSpaceRepoShouldThrow()
        {
            var resource = CreateProjectResourceForSpace("Spaces-2");
            Action activityUnderTest = () => subject.Modify(resource);
            
            activityUnderTest
                .ShouldThrow<ArgumentException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }

        [Test]
        public void Update_ResourceWithNoSpaceId_DefaultSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(null);
            Assert.DoesNotThrow(() => subject.Modify(resource));
            
        }
        
        [Test]
        public void Update_ResourceWithNoSpaceId_AnyOtherSpaceShouldThrow()
        {
            SetupRepositoryToUseADifferentSpace();
            var resource = CreateProjectResourceForSpace(null);
            Action activityUnderTest = () => subject.Modify(resource);
            
            activityUnderTest
                .ShouldThrow<ArgumentException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-2, or use a repository that is scoped to the default space.");
        }

        [Test]
        public void Delete_ResourceInExplicitSpace_NonMatchingSpaceRepoShouldThrow()
        {
            var resource = CreateProjectResourceForSpace("Spaces-2");
            Action activityUnderTest = () => subject.Delete(resource);
            
            activityUnderTest
                .ShouldThrow<ArgumentException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }
        
        [Test]
        public void Delete_ResourceInExplicitSpace_MatchingSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(defaultSpace.Id);
            Assert.DoesNotThrow(() => subject.Delete(resource));
        }
        
        [Test]
        public void Delete_ResourceWithNoSpaceId_DefaultSpaceRepoIsOk()
        {
            var resource = CreateProjectResourceForSpace(null);
            Assert.DoesNotThrow(() => subject.Delete(resource));

        }
        
        [Test]
        public void Delete_ResourceWithNoSpaceId_AnyOtherSpaceRepoShouldThrow()
        {
            SetupRepositoryToUseADifferentSpace();
            var resource = CreateProjectResourceForSpace(null);
            Action activityUnderTest = () => subject.Delete(resource);

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

        private class TestSpaceResourceRepository : BasicRepository<ProjectResource>
        {
            public TestSpaceResourceRepository(IOctopusRepository repository, string collectionLinkName, Func<IOctopusRepository, string> getCollectionLinkName = null) : base(repository, collectionLinkName, getCollectionLinkName)
            {
            }
        }
    }
}
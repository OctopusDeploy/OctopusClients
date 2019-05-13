using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Tests.Repositories.Async
{
    [TestFixture]
    public class BasicRepositoryFixture
    {
        private TestSpaceResourceAsyncRepository repoForSpaceScopedResource;
        private TestMixedResourceAsyncRepository repoForMixedScopedResource;
        private TestSystemResourceAsyncRepository repoForSystemScopedResource;
        private IOctopusAsyncRepository mockRepo;
        private SpaceResource someSpace;
        private SpaceResource otherSpace;

        [SetUp]
        public void Setup()
        {
            mockRepo = Substitute.For<IOctopusAsyncRepository>();
            
            repoForSpaceScopedResource = new TestSpaceResourceAsyncRepository(mockRepo, "", async repo => await Task.FromResult(""));
            repoForMixedScopedResource = new TestMixedResourceAsyncRepository(mockRepo, "");
            repoForSystemScopedResource = new TestSystemResourceAsyncRepository(mockRepo, "", async repo => await Task.FromResult(""));
            
            someSpace = new SpaceResource
            {
                Id = "Spaces-1",
                Name = "Some Space",
                IsDefault = false
            };

            otherSpace = new SpaceResource()
            {
                Id = "Spaces-2",
                Name = "Another space",
                IsDefault = false
            };
            
            mockRepo.Scope.Returns(RepositoryScope.ForSpace(someSpace));
        }
        
        [Test]
        public void SpaceRepo_ResourceWithSpaceIdSet_NonMatchingSpaceIdThrows()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateSpaceResourceForSpace(otherSpace.Id);
            Action activityUnderTest= () => repoForSpaceScopedResource.Create(resource).Wait();

            activityUnderTest
                .ShouldThrow<ResourceSpaceDoesNotMatchRepositorySpaceException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }
        
        [Test]
        public void SpaceRepo_ResourceWithSpaceIdSet_MatchingSpaceIdOk()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateSpaceResourceForSpace(someSpace.Id);
            Assert.DoesNotThrow(() => repoForSpaceScopedResource.Create(resource).Wait());
            resource.SpaceId.Should()
                .Be(someSpace.Id, $"the space resource {nameof(IHaveSpaceResource.SpaceId)} shouldn't have changed");
        }
        
        [Test]
        public void SpaceRepo_ResourceWithNoSpaceId_Ok()
        {
            var resource = CreateSpaceResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForSpaceScopedResource.Create(resource).Wait());
            resource.SpaceId
                .Should().Be(someSpace.Id, $"the repository scope will be used to enrich the spaceResource's {nameof(IHaveSpaceResource.SpaceId)} property");
        }
        
        [Test]
        public void SpaceRepo_MixedResourceWithSpaceIdSet_MatchingSpaceIdOk()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateMixedResourceForSpace(someSpace.Id);
            
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource).Wait());
            resource.SpaceId.Should()
                .Be(someSpace.Id, $"the space resource {nameof(IHaveSpaceResource.SpaceId)} shouldn't have changed");
        }

        [Test]
        public void SpaceRepo_MixedResourceWithSpaceIdSet_NonMatchingSpaceIdThrows()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateMixedResourceForSpace(otherSpace.Id);
            Action activityUnderTest = () => repoForMixedScopedResource.Modify(resource).Wait();
            activityUnderTest
                .ShouldThrow<ResourceSpaceDoesNotMatchRepositorySpaceException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }
        
        [Test]
        public void SpaceRepo_MixedResourceWithNoSpaceId_Ok()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateMixedResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource).Wait());
            resource.SpaceId.Should().Be(someSpace.Id,
                $"the repository scope will be used to enrich the spaceResource's {nameof(IHaveSpaceResource.SpaceId)} property");
        }

        [Test]
        public void SpaceRepo_SystemResource_Throws()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateSystemResource();
            Action activityUnderTest = () => repoForSystemScopedResource.Create(resource).Wait();
            activityUnderTest.ShouldThrow<SystemResourceIsIncompatibleWithSpaceScopedRepositoryException>();
        }

        [Test]
        public void SystemRepo_SpaceResourceWithSpaceId_Throws()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateSpaceResourceForSpace(someSpace.Id);
            Action activityUnderTest = () => repoForSpaceScopedResource.Create(resource).Wait();
            activityUnderTest.ShouldThrow<SpaceResourceIsIncompatibleWithSystemRepositoryException>();
        }
        
        [Test]
        public void SystemRepo_SpaceResourceWithNoSpaceId_Throws()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateSpaceResourceForSpace(null);
            Action activityUnderTest = () => repoForSpaceScopedResource.Create(resource).Wait();
            activityUnderTest.ShouldThrow<SpaceResourceIsIncompatibleWithSystemRepositoryException>();
        }
        
        [Test]
        public void SystemRepo_MixedResourceWithSpaceId_NonMatchingThrows()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateMixedResourceForSpace(someSpace.Id);
            Action activityUnderTest = () => repoForMixedScopedResource.Create(resource).Wait();
            activityUnderTest.ShouldThrow<ResourceSpaceDoesNotMatchRepositorySpaceException>();
        }

        [Test]
        public void SystemRepo_MixedResourceNoSpaceId_Ok()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateMixedResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource).Wait());
        }

        [Test]
        public void SystemRepo_SystemResourceNoSpaceId_Ok()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateSystemResource();
            Assert.DoesNotThrow(() => repoForSystemScopedResource.Create(resource).Wait());
        }

        [Test]
        public void UnspecifiedRepo_SpaceResourceSpaceId_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateSpaceResourceForSpace(someSpace.Id);
            Assert.DoesNotThrow(() => repoForSpaceScopedResource.Create(resource).Wait());
        }
        
        [Test]
        public void UnspecifiedRepo_SpaceResourceNoSpaceId_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateSpaceResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForSpaceScopedResource.Create(resource).Wait());
        }
        
        [Test]
        public void UnspecifiedRepo_MixedResourceWithSpaceId_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateMixedResourceForSpace(someSpace.Id);
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource).Wait());
        }
        
        [Test]
        public void UnspecifiedRepo_MixedResourceNoSpaceId_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateMixedResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource).Wait());
        }
        
        [Test]
        public void UnspecifiedRepo_SystemResource_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateSystemResource();
            Assert.DoesNotThrow(() => repoForSystemScopedResource.Create(resource).Wait());
        }

        private ProjectResource CreateSpaceResourceForSpace(string spaceId)
        {
            return new ProjectResource
            {
                Name = nameof(ProjectResource), 
                SpaceId = spaceId, 
                Links = new LinkCollection { {"Self", ""} }
            };
        }
        
        private TeamResource CreateMixedResourceForSpace(string spaceId)
        {
            return new TeamResource
            {
                Name = nameof(TeamResource), 
                SpaceId = spaceId, 
                Links = new LinkCollection { {"Self", ""} }
            };
        }
        
        private UserRoleResource CreateSystemResource()
        {
            return new UserRoleResource
            {
                Name = nameof(UserRoleResource), 
                Links = new LinkCollection { {"Self", ""} }
            };
        }

        private class TestSpaceResourceAsyncRepository : Client.Repositories.Async.BasicRepository<ProjectResource>
        {
            public TestSpaceResourceAsyncRepository(IOctopusAsyncRepository repository, string collectionLinkName, Func<IOctopusAsyncRepository, Task<string>> getCollectionLinkName = null) : base(repository, collectionLinkName, getCollectionLinkName)
            {
            }
        }
        
        private class TestMixedResourceAsyncRepository : Client.Repositories.Async.MixedScopeBaseRepository<TeamResource>
        {
            public TestMixedResourceAsyncRepository(IOctopusAsyncRepository repository, string collectionLinkName) : base(repository, collectionLinkName)
            {
            }

            protected TestMixedResourceAsyncRepository(IOctopusAsyncRepository repository, string collectionLinkName, SpaceContext userDefinedSpaceContext) : base(repository, collectionLinkName, userDefinedSpaceContext)
            {
            }
        }
        
        private class TestSystemResourceAsyncRepository : Client.Repositories.Async.BasicRepository<UserRoleResource>
        {
            public TestSystemResourceAsyncRepository(IOctopusAsyncRepository repository, string collectionLinkName, Func<IOctopusAsyncRepository, Task<string>> getCollectionLinkName = null) : base(repository, collectionLinkName, getCollectionLinkName)
            {
            }
        }
    }
    
    public static class TestExtensions
    {
        public static void SetupScopeForSpace(this IOctopusAsyncRepository repo, string space)
        {
            repo.Scope.Returns(RepositoryScope.ForSpace(new SpaceResource {Id = space, IsDefault = false}));
        }

        public static void SetupScopeForSystem(this IOctopusAsyncRepository repo)
        {
            repo.Scope.Returns(RepositoryScope.ForSystem());
        }
        
        public static void SetupScopeAsUnspecified(this IOctopusAsyncRepository repo)
        {
            repo.Scope.Returns(RepositoryScope.Unspecified());
        }
    }

}
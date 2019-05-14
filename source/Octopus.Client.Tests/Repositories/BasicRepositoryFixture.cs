using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Tests.Repositories
{
    [TestFixture]
    public class BasicRepositoryFixture
    {
        private TestSpaceResourceRepository repoForSpaceScopedResource;
        private TestMixedResourceRepository repoForMixedScopedResource;
        private TestSystemResourceRepository repoForSystemScopedResource;
        private IOctopusRepository mockRepo;
        private SpaceResource someSpace;
        private SpaceResource otherSpace;

        [SetUp]
        public void Setup()
        {
            mockRepo = Substitute.For<IOctopusRepository>();
            
            repoForSpaceScopedResource = new TestSpaceResourceRepository(mockRepo, "",  repo => "");
            repoForMixedScopedResource = new TestMixedResourceRepository(mockRepo, "");
            repoForSystemScopedResource = new TestSystemResourceRepository(mockRepo, "",  repo => "");
            
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
            Action activityUnderTest= () => repoForSpaceScopedResource.Create(resource);

            activityUnderTest
                .ShouldThrow<ResourceSpaceDoesNotMatchRepositorySpaceException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }
        
        [Test]
        public void SpaceRepo_ResourceWithSpaceIdSet_MatchingSpaceIdOk()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateSpaceResourceForSpace(someSpace.Id);
            Assert.DoesNotThrow(() => repoForSpaceScopedResource.Create(resource));
            resource.SpaceId.Should()
                .Be(someSpace.Id, $"the space resource {nameof(IHaveSpaceResource.SpaceId)} shouldn't have changed");
        }
        
        [Test]
        public void SpaceRepo_ResourceWithNoSpaceId_Ok()
        {
            var resource = CreateSpaceResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForSpaceScopedResource.Create(resource));
            resource.SpaceId
                .Should().Be(someSpace.Id, $"the repository scope will be used to enrich the spaceResource's {nameof(IHaveSpaceResource.SpaceId)} property");
        }
        
        [Test]
        public void SpaceRepo_MixedResourceWithSpaceIdSet_MatchingSpaceIdOk()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateMixedResourceForSpace(someSpace.Id);
            
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource));
            resource.SpaceId.Should()
                .Be(someSpace.Id, $"the space resource {nameof(IHaveSpaceResource.SpaceId)} shouldn't have changed");
        }

        [Test]
        public void SpaceRepo_MixedResourceWithSpaceIdSet_NonMatchingSpaceIdThrows()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateMixedResourceForSpace(otherSpace.Id);
            Action activityUnderTest = () => repoForMixedScopedResource.Modify(resource);
            activityUnderTest
                .ShouldThrow<ResourceSpaceDoesNotMatchRepositorySpaceException>()
                .WithMessage("The resource has a different space specified than the one specified by the repository scope. Either change the SpaceId on the resource to Spaces-1, or use a repository that is scoped to Spaces-2.");
        }
        
        [Test]
        public void SpaceRepo_MixedResourceWithNoSpaceId_Ok()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateMixedResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource));
            resource.SpaceId.Should().Be(someSpace.Id,
                $"the repository scope will be used to enrich the spaceResource's {nameof(IHaveSpaceResource.SpaceId)} property");
        }

        [Test]
        public void SpaceRepo_SystemResource_Throws()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            var resource = CreateSystemResource();
            Action activityUnderTest = () => repoForSystemScopedResource.Create(resource);
            activityUnderTest.ShouldThrow<SystemResourceIsIncompatibleWithSpaceScopedRepositoryException>();
        }
        
        [Test]
        public void SpaceRepo_GetSystemResource_Throws()
        {
            mockRepo.SetupScopeForSpace(someSpace.Id);
            Action activityUnderTest = () => repoForSystemScopedResource.GetAll();
            activityUnderTest.ShouldThrow<SystemResourceIsIncompatibleWithSpaceScopedRepositoryException>("GetAll operations that attempt to access system resources when the repository is scoped to a space should fail.");
            
            activityUnderTest = () => repoForSystemScopedResource.Get();
            activityUnderTest.ShouldThrow<SystemResourceIsIncompatibleWithSpaceScopedRepositoryException>("Get operations that attempt to access system resources when the repository is scoped to a space should fail.");
            
            activityUnderTest = () => repoForSystemScopedResource.Get("");
            activityUnderTest.ShouldThrow<SystemResourceIsIncompatibleWithSpaceScopedRepositoryException>("Get(idOrHref) operations that attempt to access system resources when the repository is scoped to a space should fail.");
        }

        [Test]
        public void SystemRepo_SpaceResourceWithSpaceId_Throws()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateSpaceResourceForSpace(someSpace.Id);
            Action activityUnderTest = () => repoForSpaceScopedResource.Create(resource);
            activityUnderTest.ShouldThrow<SpaceResourceIsIncompatibleWithSystemRepositoryException>();
        }
        
        [Test]
        public void SystemRepo_GetSpaceResource_Throws()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateSpaceResourceForSpace(null);
            Action activityUnderTest = () => repoForSpaceScopedResource.GetAll();
            activityUnderTest.ShouldThrow<SpaceResourceIsIncompatibleWithSystemRepositoryException>(because: "GetAll operations that attempt to access space resources when the repository is scoped to system should fail.");
            
            activityUnderTest = () => repoForSpaceScopedResource.Get();
            activityUnderTest.ShouldThrow<SpaceResourceIsIncompatibleWithSystemRepositoryException>(because: "Get operations that attempt to access space resources when the repository is scoped to system should fail.");
            
            activityUnderTest = () => repoForSpaceScopedResource.Get("");
            activityUnderTest.ShouldThrow<SpaceResourceIsIncompatibleWithSystemRepositoryException>(because: "Get(idOrHref) operations that attempt to access space resources when the repository is scoped to system should fail.");
        }
        
        [Test]
        public void SystemRepo_SpaceResourceWithNoSpaceId_Throws()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateSpaceResourceForSpace(null);
            Action activityUnderTest = () => repoForSpaceScopedResource.Create(resource);
            activityUnderTest.ShouldThrow<SpaceResourceIsIncompatibleWithSystemRepositoryException>();
        }
        
        [Test]
        public void SystemRepo_MixedResourceWithSpaceId_NonMatchingThrows()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateMixedResourceForSpace(someSpace.Id);
            Action activityUnderTest = () => repoForMixedScopedResource.Create(resource);
            activityUnderTest.ShouldThrow<SpaceResourceIsIncompatibleWithSystemRepositoryException>();
        }

        [Test]
        public void SystemRepo_MixedResourceNoSpaceId_Ok()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateMixedResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource));
        }

        [Test]
        public void SystemRepo_SystemResourceNoSpaceId_Ok()
        {
            mockRepo.SetupScopeForSystem();
            var resource = CreateSystemResource();
            Assert.DoesNotThrow(() => repoForSystemScopedResource.Create(resource));
        }

        [Test]
        public void UnspecifiedRepo_SpaceResourceSpaceId_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateSpaceResourceForSpace(someSpace.Id);
            Assert.DoesNotThrow(() => repoForSpaceScopedResource.Create(resource));
        }
        
        [Test]
        public void UnspecifiedRepo_SpaceResourceNoSpaceId_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateSpaceResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForSpaceScopedResource.Create(resource));
        }
        
        [Test]
        public void UnspecifiedRepo_SpaceResourceNoSpaceIdDefaultSpaceMissing_Throws()
        {
            mockRepo.SetupScopeAsUnspecifiedWithDefaultSpaceDisabled();
            var resource = CreateSpaceResourceForSpace(null);
            Action actionUnderTest = () => repoForSpaceScopedResource.Create(resource);
            actionUnderTest.ShouldThrow<DefaultSpaceNotFoundException>();
        }
        
        [Test]
        public void UnspecifiedRepo_MixedResourceWithSpaceId_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateMixedResourceForSpace(someSpace.Id);
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource));
        }
        
        [Test]
        public void UnspecifiedRepo_MixedResourceNoSpaceId_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateMixedResourceForSpace(null);
            Assert.DoesNotThrow(() => repoForMixedScopedResource.Create(resource));
        }
        
        [Test]
        public void UnspecifiedRepo_SystemResource_Ok()
        {
            mockRepo.SetupScopeAsUnspecified();
            var resource = CreateSystemResource();
            Assert.DoesNotThrow(() => repoForSystemScopedResource.Create(resource));
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

        private class TestSpaceResourceRepository : BasicRepository<ProjectResource>
        {
            public TestSpaceResourceRepository(IOctopusRepository repository, string collectionLinkName, Func<IOctopusRepository, string> getCollectionLinkName = null) : base(repository, collectionLinkName, getCollectionLinkName)
            {
            }
        }
        
        private class TestMixedResourceRepository : MixedScopeBaseRepository<TeamResource>
        {
            public TestMixedResourceRepository(IOctopusRepository repository, string collectionLinkName) : base(repository, collectionLinkName)
            {
            }

            protected TestMixedResourceRepository(IOctopusRepository repository, string collectionLinkName, SpaceContext userDefinedSpaceContext) : base(repository, collectionLinkName, userDefinedSpaceContext)
            {
            }
        }
        
        private class TestSystemResourceRepository : BasicRepository<UserRoleResource>
        {
            public TestSystemResourceRepository(IOctopusRepository repository, string collectionLinkName, Func<IOctopusRepository, string> getCollectionLinkName = null) : base(repository, collectionLinkName, getCollectionLinkName)
            {
            }
        }
    }
    
    public static class TestExtensions
    {
        public static void SetupScopeForSpace(this IOctopusRepository repo, string space)
        {
            repo.Scope.Returns(RepositoryScope.ForSpace(new SpaceResource {Id = space, IsDefault = false}));
        }

        public static void SetupScopeForSystem(this IOctopusRepository repo)
        {
            repo.Scope.Returns(RepositoryScope.ForSystem());
        }
        
        public static void SetupScopeAsUnspecified(this IOctopusRepository repo)
        {
            repo.Scope.Returns(RepositoryScope.Unspecified());
            repo.LoadSpaceRootDocument().Returns(new SpaceRootResource());
        }
        
        public static void SetupScopeAsUnspecifiedWithDefaultSpaceDisabled(this IOctopusRepository repo)
        {
            repo.Scope.Returns(RepositoryScope.Unspecified());
            repo.LoadSpaceRootDocument().Returns((SpaceRootResource)null);
        }
    }
}
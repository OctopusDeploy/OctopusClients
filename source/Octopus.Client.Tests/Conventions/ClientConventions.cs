using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Octopus.Client.Extensions;
using Octopus.Client.Model;
using Autofac;
using FluentAssertions;
using Nancy.Extensions;
using Octopus.Client.Extensibility;
using Octopus.Client.Repositories.Async;
using Sync = Octopus.Client.Repositories;

#if HAS_BEST_CONVENTIONAL
using Conventional;
using Conventional.Conventions;
#endif

namespace Octopus.Client.Tests.Conventions
{
    [TestFixture]
    public class ClientConventions
    {
        private static readonly TypeInfo[] ExportedTypes = typeof(IOctopusAsyncClient).GetTypeInfo().Assembly.GetExportedTypes().Select(t => t.GetTypeInfo()).ToArray();

        private static readonly TypeInfo[] RepositoryInterfaceTypes = ExportedTypes
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .Where(t => t.AsType() != typeof(IOctopusAsyncRepository) && t.AsType() != typeof(IResourceRepository))
#if SYNC_CLIENT
            .Where(t => t.AsType() != typeof(IOctopusRepository) && t.AsType() != typeof(Sync.IResourceRepository))
#endif
            .ToArray();

        static readonly TypeInfo[] AsyncRepositoryInterfaceTypes = RepositoryInterfaceTypes.Where(i => i.Namespace.EndsWith(".Async")).ToArray();
        static readonly TypeInfo[] SyncRepositoryInterfaceTypes = RepositoryInterfaceTypes.Except(AsyncRepositoryInterfaceTypes).ToArray();

        private static readonly TypeInfo[] RepositoryTypes = ExportedTypes
            .Where(t => !t.IsInterface && t.Name.EndsWith("Repository"))
            .Where(t => t.AsType() != typeof(OctopusAsyncRepository))
            .ToArray();

        private static readonly TypeInfo[] ResourceTypes = ExportedTypes
            .Where(t => !t.IsInterface && !t.IsAbstract && t.Name.EndsWith("Resource"))
            .ToArray();

        private static readonly TypeInfo[] RepositoryResourceTypes = ResourceTypes
            .Where(res => RepositoryTypes
                .Any(rep => rep.BaseType?.GetTypeInfo().IsGenericType == true && rep.BaseType?.GetTypeInfo().GetGenericArguments().Contains(res.AsType()) == true))
            .ToArray();

        [Test]
        public void AllAsyncRepositoriesShouldBeAvailableViaIOctopusAsyncRepository()
        {
            var exposedTypes = typeof(IOctopusAsyncRepository).GetProperties()
                .Select(p => p.PropertyType.GetTypeInfo())
                .ToArray();

            var missingTypes = AsyncRepositoryInterfaceTypes.Except(exposedTypes).ToArray();
            if (missingTypes.Any())
            {
                Assert.Fail($"All async repository types should be exposed by {nameof(IOctopusAsyncRepository)}. Missing: {string.Join(", ", missingTypes.Select(t => t.Name))}");
            }
        }

        [Test]
        public void ThereShouldBeAsyncRepositories()
        {
            AsyncRepositoryInterfaceTypes.Should().NotBeEmpty();
        }

#if SYNC_CLIENT
        [Test]
        public void ThereShouldBeSyncRepositories()
        {
            SyncRepositoryInterfaceTypes.Should().NotBeEmpty();
        }
#else
        [Test]
        public void ThereShouldBeNoSyncRepositories()
        {
            SyncRepositoryInterfaceTypes.Should().BeEmpty();
        }
#endif

#if SYNC_CLIENT

        [Test]
        public void AllSyncRepositoriesShouldBeAvailableViaIOctopusRepository()
        {
            var exposedTypes = typeof(IOctopusRepository).GetProperties()
                .Select(p => p.PropertyType.GetTypeInfo())
                .ToArray();

            var missingTypes = SyncRepositoryInterfaceTypes.Except(exposedTypes).ToArray();
            if (missingTypes.Any())
            {
                Assert.Fail($"All sync *Repository types should be exposed by {nameof(IOctopusRepository)}. Missing: {string.Join(", ", missingTypes.Select(t => t.Name))}");
            }
        }
#endif


        [Test]
        public void AllRepositoriesShouldImplementNonGenericSimpleInterface()
        {
            var repositoryInterfaceMap = RepositoryTypes.Select(r => new { Repository = r, Interface = r.GetInterfaces().Where(i => !i.GetTypeInfo().IsGenericType) }).ToArray();
            var missingInterface = repositoryInterfaceMap.Where(x => x.Interface == null).ToArray();

            if (missingInterface.Any())
            {
                Assert.Fail($"All *Repository types should implement a non-generic interface representing the repository contract {nameof(OctopusAsyncRepository)}.{Environment.NewLine}{missingInterface.Select(x => $"{x.Repository.Name} expected to implement I{x.Repository.Name}").NewLineSeperate()}");
            }
        }

        [Test]
        public void AllRepositoriesShouldExposePublicMembersViaTheirInterfaces()
        {
            var exposureMap = RepositoryTypes
                .Select(r => new
                {
                    Repository = r,
                    DeclaredMethodMap = r.GetMethods()
                    .Where(m => m.DeclaringType.GetTypeInfo() == r)
                    .Select(m => new
                    {
                        DelcaredMethod = m,
                        r.GetInterfaces().Select(r.GetRuntimeInterfaceMap).FirstOrDefault(map => map.TargetMethods.Contains(m)).InterfaceType
                    }).ToArray(),
                }).ToArray();

            var missingExposure = exposureMap.Where(x => x.DeclaredMethodMap.Any(map => map.InterfaceType == null)).ToArray();
            if (missingExposure.Any())
            {
                Assert.Fail($"The following repositories do not expose at least one of their public members by interface, and hence won't be accessible by clients.{Environment.NewLine}{exposureMap.Where(x => x.DeclaredMethodMap.Any(map => map.InterfaceType == null)).Select(x => $"{x.Repository.Name}: {x.DeclaredMethodMap.Where(map => map.InterfaceType == null).Select(map => $"{map.DelcaredMethod.Name}({map.DelcaredMethod.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}").CommaSeperate()})").CommaSeperate()}").NewLineSeperate()}");
            }
        }

        [Test]
        public void RepositoriesShouldNotRepresentMultipleResourceTypes()
        {
            var repositoryResourceMap = RepositoryTypes
                .Select(t => new
                {
                    Repository = t,
                    ResourceTypes = t.GetInterfaces().Concat(new[] { t.BaseType })
                        .Where(i => i.GetTypeInfo().IsGenericType)
                        .SelectMany(i => i.GetGenericArguments())
                        .Distinct()
                        .ToArray()
                }).ToArray();

            var confusedRepositories = repositoryResourceMap.Where(x => x.ResourceTypes.Length > 1).ToArray();

            if (confusedRepositories.Any())
            {
                Assert.Fail($"Repositories should represent consistent Resource type. These repositories have an identity crisis: {Environment.NewLine}{confusedRepositories.Select(x => $"{x.Repository.Name}<{x.ResourceTypes.Select(r => r.Name).CommaSeperate()}>").NewLineSeperate()}");
            }
        }

        [Test]
        public void RepositoriesShouldBeNamedLikeTheirResourceType()
        {
            var repositoryResourceMap = RepositoryTypes
                .Select(t => new
                {
                    Repository = t,
                    ResourceTypes = t.GetInterfaces().Concat(new[] { t.BaseType })
                        .Where(i => i.GetTypeInfo().IsGenericType)
                        .SelectMany(i => i.GetGenericArguments())
                        .Distinct()
                        .ToArray()
                }).ToArray();

            var confusingRepositories = repositoryResourceMap
                .Where(x => x.ResourceTypes.Any())
                .Where(x => !x.Repository.Name.StartsWith(x.ResourceTypes.First().Name.Replace("Resource", "")))
                .ToArray();

            if (confusingRepositories.Any())
            {
                Assert.Fail($"Repositories should be named like their Resource type. These repositories could be confusing: {Environment.NewLine}{confusingRepositories.Select(x => $"{x.Repository.Name}<{x.ResourceTypes.Select(r => r.Name).CommaSeperate()}> - based on the resource type this should be named something like {x.ResourceTypes.First().Name.Replace("Resource", "")}Repository? Or maybe this is using the wrong generic type argument?").NewLineSeperate()}");
            }
        }

        [Test]
        public void TopLevelResourcesWithAPublicNamePropertyShouldProbablyImplementINamedResource()
        {
            var ignored = new[]
            {
                typeof (DeploymentResource).GetTypeInfo(),
                typeof (TaskResource).GetTypeInfo()
            };

            var shouldProbablyBeINamedResource = RepositoryResourceTypes
                .Except(ignored)
                .Where(t => t.GetProperty("Name") != null && !t.AsType().IsAssignableTo<INamedResource>())
                .ToArray();

            if (shouldProbablyBeINamedResource.Any())
            {
                Assert.Fail($"The following top-level resource types have a Name property, and should probably implement INamedResource: {Environment.NewLine}{shouldProbablyBeINamedResource.Select(t => t.Name).NewLineSeperate()}");
            }
        }

        [Test]
        public void SomeINamedResourcesShouldNeverBeIFindByNameToAvoidGettingTheWrongAnswer()
        {
            var denied = new[]
            {
#if SYNC_CLIENT
                typeof (Sync.IChannelRepository),
                typeof (Sync.IDeploymentProcessRepository),
                typeof (Sync.ITaskRepository),
#endif
                typeof (IChannelRepository),
                typeof (IDeploymentProcessRepository),
                typeof (ITaskRepository)
            };

            var misleadingRepositories = denied.Where(r => r.IsAssignableToGenericType(typeof(IFindByName<>))).ToArray();

            if (misleadingRepositories.Any())
            {
                Assert.Fail($"The following repositories allow the client to FindByName, but this will end up returning a misleading result, and the resource should be loaded in another way:{Environment.NewLine}{misleadingRepositories.Select(r => $"{r.Name}").NewLineSeperate()}");
            }
        }

        [Test]
        public void AsyncRepositoriesThatGetNamedResourcesShouldUsuallyImplementIFindByName()
        {
            var ignored = new[]
            {
                typeof (IChannelRepository).GetTypeInfo(),
                typeof (IProjectTriggerRepository).GetTypeInfo()
            };

            var getsResources = AsyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(IGet<>)))
                .ToArray();

            var getsNamedResources = getsResources
                .Where(t => t.GetInterfaces()
                    .Where(i => i.IsClosedTypeOf(typeof(IGet<>)))
                    .Any(i => i.GenericTypeArguments.Any(r => r.IsAssignableTo<INamedResource>())))
                .ToArray();

            var canFindByName = getsNamedResources
                .Where(t => t.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(IFindByName<>))))
                .ToArray();

            var missingFindByName = getsNamedResources.Except(canFindByName).ToArray();

            if (missingFindByName.Any())
            {
                Assert.Fail($"Repositories that implement IGet<INamedResource> should usually implement IFindByName<INamedResource>, unless that named resource is a singleton or owned by another aggregate.{Environment.NewLine}{missingFindByName.Select(t => t.Name).NewLineSeperate()}");
            }
        }

#if SYNC_CLIENT
        [Test]
        public void SyncRepositoriesThatGetNamedResourcesShouldUsuallyImplementIFindByName()
        {
            var ignored = new[]
            {
                typeof (Sync.IChannelRepository).GetTypeInfo(),
                typeof (Sync.IProjectTriggerRepository).GetTypeInfo()
            };

            var getsResources = SyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(Sync.IGet<>)))
                .ToArray();

            var getsNamedResources = getsResources
                .Where(t => t.GetInterfaces()
                    .Where(i => i.IsClosedTypeOf(typeof(Sync.IGet<>)))
                    .Any(i => i.GenericTypeArguments.Any(r => r.IsAssignableTo<INamedResource>())))
                .ToArray();

            var canFindByName = getsNamedResources
                .Where(t => t.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(Sync.IFindByName<>))))
                .ToArray();

            var missingFindByName = getsNamedResources.Except(canFindByName).ToArray();

            if (missingFindByName.Any())
            {
                Assert.Fail($"Repositories that implement IGet<INamedResource> should usually implement IFindByName<INamedResource>, unless that named resource is a singleton or owned by another aggregate.{Environment.NewLine}{missingFindByName.Select(t => t.Name).NewLineSeperate()}");
            }
        }
#endif

        [Test]
        public void MostAsyncRepositoriesThatGetResourcesShouldImplementIPaginate()
        {
            var ignored = new[]
            {
                typeof (IDeploymentProcessRepository).GetTypeInfo(),
                typeof (IInterruptionRepository).GetTypeInfo(),
                typeof (IEventRepository).GetTypeInfo(),
                typeof (IVariableSetRepository).GetTypeInfo(),
                typeof (IChannelRepository).GetTypeInfo(),
                typeof (IProjectTriggerRepository).GetTypeInfo(),
                typeof (ICommunityActionTemplateRepository).GetTypeInfo(),
                typeof (IScopedUserRoleRepository).GetTypeInfo()
            };

            var missing = AsyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(IGet<>)))
                .Except(RepositoryInterfaceTypes.Where(t => t.AsType().IsAssignableToGenericType(typeof(IPaginate<>))))
                .ToArray();

            if (missing.Any())
            {
                Assert.Fail($"Most repositories that get resources should implement IPaginate<TResource> unless the repository should target one specific resource like a singleton or child of another aggregate.{Environment.NewLine}{missing.Select(t => t.Name).NewLineSeperate()}");
            }
        }

#if SYNC_CLIENT
        [Test]
        public void MostSyncRepositoriesThatGetResourcesShouldImplementIPaginate()
        {
            var ignored = new[]
            {
                typeof (Sync.IDeploymentProcessRepository).GetTypeInfo(),
                typeof (Sync.IInterruptionRepository).GetTypeInfo(),
                typeof (Sync.IEventRepository).GetTypeInfo(),
                typeof (Sync.IVariableSetRepository).GetTypeInfo(),
                typeof (Sync.IChannelRepository).GetTypeInfo(),
                typeof (Sync.IProjectTriggerRepository).GetTypeInfo(),
                typeof (Sync.ICommunityActionTemplateRepository).GetTypeInfo(),
                typeof (Sync.IScopedUserRoleRepository).GetTypeInfo()
            };

            var missing = SyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(Sync.IGet<>)))
                .Except(RepositoryInterfaceTypes.Where(t => t.AsType().IsAssignableToGenericType(typeof(Sync.IPaginate<>))))
                .ToArray();

            if (missing.Any())
            {
                Assert.Fail($"Most repositories that get resources should implement IPaginate<TResource> unless the repository should target one specific resource like a singleton or child of another aggregate.{Environment.NewLine}{missing.Select(t => t.Name).NewLineSeperate()}");
            }
        }
#endif

        [Test]
        public void AsyncRepositoriesThatImplementCreateShouldAlsoImplementModify()
        {
            var ignored = new []
            {
                typeof(IDeploymentRepository).GetTypeInfo(),
                typeof(ITaskRepository).GetTypeInfo()
            };

            var createsResources = AsyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(ICreate<>)))
                .ToArray();

            var alsoImplementsModify = createsResources
                .Where(t => t.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(IModify<>))))
                .ToArray();

            var missingModify = createsResources.Except(alsoImplementsModify).ToArray();

            if (missingModify.Any())
            {
                Assert.Fail($"Repositories that implement ICreate<IResource> should usually implement IModify<IResource>.{Environment.NewLine}{missingModify.Select(t => t.Name).NewLineSeperate()}");
            }
        }

#if SYNC_CLIENT
        [Test]
        public void SyncRepositoriesThatImplementCreateShouldAlsoImplementModify()
        {
            var ignored = new[]
            {
                typeof(Sync.IDeploymentRepository).GetTypeInfo(),
                typeof(Sync.ITaskRepository).GetTypeInfo()
            };

            var createsResources = SyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(ICreate<>)))
                .ToArray();

            var alsoImplementsModify = createsResources
                .Where(t => t.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(IModify<>))))
                .ToArray();

            var missingModify = createsResources.Except(alsoImplementsModify).ToArray();

            if (missingModify.Any())
            {
                Assert.Fail($"Repositories that implement ICreate<IResource> should usually implement IModify<IResource>.{Environment.NewLine}{missingModify.Select(t => t.Name).NewLineSeperate()}");
            }
        }
#endif

        [Test]
        public void AsyncRepositoriesThatImplementCreateShouldAlsoImplementDelete()
        {
            var ignored = new[]
            {
                typeof(IDeploymentRepository).GetTypeInfo(),
                typeof(ITaskRepository).GetTypeInfo()
            };

            var createsResources = AsyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(ICreate<>)))
                .ToArray();

            var alsoImplementsDelete = createsResources
                .Where(t => t.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(IDelete<>))))
                .ToArray();

            var missingDelete = createsResources.Except(alsoImplementsDelete).ToArray();

            if (missingDelete.Any())
            {
                Assert.Fail($"Repositories that implement ICreate<IResource> should usually implement IDelete<IResource>.{Environment.NewLine}{missingDelete.Select(t => t.Name).NewLineSeperate()}");
            }
        }

#if SYNC_CLIENT
        [Test]
        public void SyncRepositoriesThatImplementCreateShouldAlsoImplementDelete()
        {
            var ignored = new[]
            {
                typeof(Sync.IDeploymentRepository).GetTypeInfo(),
                typeof(Sync.ITaskRepository).GetTypeInfo()
            };

            var createsResources = SyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(ICreate<>)))
                .ToArray();

            var alsoImplementsDelete = createsResources
                .Where(t => t.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(IDelete<>))))
                .ToArray();

            var missingDelete = createsResources.Except(alsoImplementsDelete).ToArray();

            if (missingDelete.Any())
            {
                Assert.Fail($"Repositories that implement ICreate<IResource> should usually implement IDelete<IResource>.{Environment.NewLine}{missingDelete.Select(t => t.Name).NewLineSeperate()}");
            }
        }
#endif

#if HAS_BEST_CONVENTIONAL

        [Test]
        public void AllSyncRepositoryInterfacesShouldFollowTheseConventions()
        {
            SyncRepositoryInterfaceTypes
                .MustConformTo(Convention.MustLiveInNamespace("Octopus.Client.Repositories"))
                .AndMustConformTo(Convention.NameMustEndWith("Repository"))
                .WithFailureAssertion(Assert.Fail);
        }

        [Test]
        public void AllAsyncRepositoryInterfacesShouldFollowTheseConventions()
        {
            AsyncRepositoryInterfaceTypes
                .MustConformTo(Convention.MustLiveInNamespace("Octopus.Client.Repositories.Async"))
                .AndMustConformTo(Convention.NameMustEndWith("Repository"))
                .WithFailureAssertion(Assert.Fail);
        }


        [Test]
        public void AllResourcesShouldLiveInTheCorrectParentNamespace()
        {
            ResourceTypes
                .MustConformTo(new MustLiveInParentNamespaceConventionSpecification("Octopus.Client.Model"))
                .WithFailureAssertion(Assert.Fail);
        }

        [Test]
        public void AllResourcePropertiesShouldHavePublicGetters()
        {
            ResourceTypes
                .MustConformTo(Convention.PropertiesMustHavePublicGetters)
                .WithFailureAssertion(Assert.Fail);
        }

        [Test]
        public void AllResourcePropertiesShouldHavePublicSetters()
        {
            ResourceTypes
                .Except(new[] { typeof(LifecycleResource), typeof(DeploymentProcessResource), typeof(CertificateResource) })
                .MustConformTo(Convention.PropertiesMustHavePublicSetters)
                .WithFailureAssertion(Assert.Fail);
        }

        public class MustLiveInParentNamespaceConventionSpecification : ConventionSpecification
        {
            private readonly string parentNamespace;

            protected override string FailureMessage => "Must live in parent namespace {0} but actually lives in namespace {1}";

            public MustLiveInParentNamespaceConventionSpecification(string parentNamespace)
            {
                this.parentNamespace = parentNamespace;
            }

            public override ConventionResult IsSatisfiedBy(Type type)
            {
                if (type.Namespace != null && type.Namespace.StartsWith(parentNamespace))
                    return ConventionResult.Satisfied(type.FullName);
                return ConventionResult.NotSatisfied(type.FullName, string.Format(FailureMessage, parentNamespace, type.Namespace));
            }
        }
#endif
    }
}
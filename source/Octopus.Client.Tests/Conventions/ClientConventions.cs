using System;
using System.Collections.Generic;
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
using Conventional;
using Conventional.Conventions;

namespace Octopus.Client.Tests.Conventions
{
    [TestFixture]
    public class ClientConventions
    {
        private static readonly TypeInfo[] ExportedTypes = typeof(IOctopusAsyncClient).GetTypeInfo().Assembly.GetExportedTypes().Select(t => t.GetTypeInfo()).ToArray();

        private static readonly TypeInfo[] RepositoryInterfaceTypes = ExportedTypes
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .Where(t => t.AsType() != typeof(IOctopusAsyncRepository) && t.AsType() != typeof(IResourceRepository))
            .Where(t => t.AsType() != typeof(IOctopusSpaceAsyncRepository))
            .Where(t => t.AsType() != typeof(IOctopusSystemAsyncRepository))
            .Where(t => t.AsType() != typeof(IOctopusCommonAsyncRepository))
            .Where(t => t.AsType() != typeof(IOctopusRepository) && t.AsType() != typeof(Sync.IResourceRepository))
            .Where(t => t.AsType() != typeof(IOctopusSpaceRepository))
            .Where(t => t.AsType() != typeof(IOctopusSystemRepository))
            .Where(t => t.AsType() != typeof(IOctopusCommonRepository))
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
            var exposedTypes = GetRepoTypesReachableFrom(typeof(IOctopusAsyncRepository));

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

        [Test]
        public void AllSyncRepositoriesShouldBeAvailableViaIOctopusRepository()
        {
            var exposedTypes = GetRepoTypesReachableFrom(typeof(IOctopusRepository));
            
            var missingTypes = SyncRepositoryInterfaceTypes.Except(exposedTypes).ToArray();
            if (missingTypes.Any())
            {
                Assert.Fail($"All sync *Repository types should be exposed by {nameof(IOctopusRepository)}. Missing: {string.Join(", ", missingTypes.Select(t => t.Name))}");
            }
        }

        private static ISet<Type> GetRepoTypesReachableFrom(Type root)
        {
            var visitedSoFar = new HashSet<Type>();
            RecursivelyCollectRepositoryTypes(root, visitedSoFar);
            return visitedSoFar;
        }
        
        private static void RecursivelyCollectRepositoryTypes(Type root, ISet<Type> visitedSoFar)
        {
            var repoAssembly = typeof(IOctopusRepository).Assembly;
            
            var interfaces = root.GetInterfaces()
                .Concat(new[] {root})
                .ToArray();
            
            var newTypesExposedViaProperty = interfaces
                .SelectMany(i => i.GetProperties())
                .Select(p => p.PropertyType.GetTypeInfo())
                .Where(p => p.Assembly == repoAssembly)
                .ToArray();
            
            var newTypesExposedViaMethod = interfaces
                .SelectMany(i => i.GetMethods())
                .Select(p => p.ReturnType.GetTypeInfo())
                .Where(p => p.Assembly == repoAssembly)
                .ToArray();

            var newExposedRepositoryTypes = newTypesExposedViaProperty
                .Concat(newTypesExposedViaMethod)
                .Where(t => t.Name.EndsWith("Repository"))
                .Except(visitedSoFar)
                .ToArray();

            foreach (var type in newExposedRepositoryTypes)
            {
                visitedSoFar.Add(type);
            }

            foreach (var type in newExposedRepositoryTypes)
            {
                RecursivelyCollectRepositoryTypes(type, visitedSoFar);
            }
        }
        
        [Test]
        public void AllRepositoriesShouldImplementNonGenericSimpleInterface()
        {
            var repositoryInterfaceMap = RepositoryTypes.Select(r => new { Repository = r, Interface = r.GetInterfaces().Where(i => !i.GetTypeInfo().IsGenericType) }).ToArray();
            var missingInterface = repositoryInterfaceMap.Where(x => x.Interface == null).ToArray();

            if (missingInterface.Any())
            {
                Assert.Fail($"All *Repository types should implement a non-generic interface representing the repository contract {nameof(OctopusAsyncRepository)}.{Environment.NewLine}{missingInterface.Select(x => $"{x.Repository.Name} expected to implement I{x.Repository.Name}").NewLineSeparate()}");
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
                Assert.Fail($"The following repositories do not expose at least one of their public members by interface, and hence won't be accessible by clients.{Environment.NewLine}{exposureMap.Where(x => x.DeclaredMethodMap.Any(map => map.InterfaceType == null)).Select(x => $"{x.Repository.Name}: {x.DeclaredMethodMap.Where(map => map.InterfaceType == null).Select(map => $"{map.DelcaredMethod.Name}({map.DelcaredMethod.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}").CommaSeparate()})").CommaSeparate()}").NewLineSeparate()}");
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
                Assert.Fail($"Repositories should represent consistent Resource type. These repositories have an identity crisis: {Environment.NewLine}{confusedRepositories.Select(x => $"{x.Repository.Name}<{x.ResourceTypes.Select(r => r.Name).CommaSeparate()}>").NewLineSeparate()}");
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
                Assert.Fail($"Repositories should be named like their Resource type. These repositories could be confusing: {Environment.NewLine}{confusingRepositories.Select(x => $"{x.Repository.Name}<{x.ResourceTypes.Select(r => r.Name).CommaSeparate()}> - based on the resource type this should be named something like {x.ResourceTypes.First().Name.Replace("Resource", "")}Repository? Or maybe this is using the wrong generic type argument?").NewLineSeparate()}");
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
                Assert.Fail($"The following top-level resource types have a Name property, and should probably implement INamedResource: {Environment.NewLine}{shouldProbablyBeINamedResource.Select(t => t.Name).NewLineSeparate()}");
            }
        }

        [Test]
        public void SomeINamedResourcesShouldNeverBeIFindByNameToAvoidGettingTheWrongAnswer()
        {
            var denied = new[]
            {
                typeof (Sync.IChannelRepository),
                typeof (Sync.IRunbookProcessRepository),
                typeof (Sync.IDeploymentProcessRepository),
                typeof (Sync.ITaskRepository),
                typeof (IChannelRepository),
                typeof (IRunbookProcessRepository),
                typeof (IDeploymentProcessRepository),
                typeof (ITaskRepository)
            };

            var misleadingRepositories = denied.Where(r => r.IsAssignableToGenericType(typeof(IFindByName<>))).ToArray();

            if (misleadingRepositories.Any())
            {
                Assert.Fail($"The following repositories allow the client to FindByName, but this will end up returning a misleading result, and the resource should be loaded in another way:{Environment.NewLine}{misleadingRepositories.Select(r => $"{r.Name}").NewLineSeparate()}");
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
                    .Where(i => Autofac.TypeExtensions.IsClosedTypeOf(i, typeof(IGet<>)))
                    .Any(i => i.GenericTypeArguments.Any(r => r.IsAssignableTo<INamedResource>())))
                .ToArray();

            var canFindByName = getsNamedResources
                .Where(t => t.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(IFindByName<>))))
                .ToArray();

            var missingFindByName = getsNamedResources.Except(canFindByName).ToArray();

            if (missingFindByName.Any())
            {
                Assert.Fail($"Repositories that implement IGet<INamedResource> should usually implement IFindByName<INamedResource>, unless that named resource is a singleton or owned by another aggregate.{Environment.NewLine}{missingFindByName.Select(t => t.Name).NewLineSeparate()}");
            }
        }

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
                Assert.Fail($"Repositories that implement IGet<INamedResource> should usually implement IFindByName<INamedResource>, unless that named resource is a singleton or owned by another aggregate.{Environment.NewLine}{missingFindByName.Select(t => t.Name).NewLineSeparate()}");
            }
        }

        [Test]
        public void MostAsyncRepositoriesThatGetResourcesShouldImplementIPaginate()
        {
            var ignored = new[]
            {
                typeof (IRunbookProcessRepository).GetTypeInfo(),
                typeof (IDeploymentProcessRepository).GetTypeInfo(),
                typeof (IInterruptionRepository).GetTypeInfo(),
                typeof (IEventRepository).GetTypeInfo(),
                typeof (IVariableSetRepository).GetTypeInfo(),
                typeof (IChannelRepository).GetTypeInfo(),
                typeof (IProjectTriggerRepository).GetTypeInfo(),
                typeof (ICommunityActionTemplateRepository).GetTypeInfo(),
                typeof (IScopedUserRoleRepository).GetTypeInfo(),
                typeof (IUpgradeConfigurationRepository).GetTypeInfo()
            };

            var missing = AsyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(IGet<>)))
                .Except(RepositoryInterfaceTypes.Where(t => t.AsType().IsAssignableToGenericType(typeof(IPaginate<>))))
                .ToArray();

            if (missing.Any())
            {
                Assert.Fail($"Most repositories that get resources should implement IPaginate<TResource> unless the repository should target one specific resource like a singleton or child of another aggregate.{Environment.NewLine}{missing.Select(t => t.Name).NewLineSeparate()}");
            }
        }

        [Test]
        public void MostSyncRepositoriesThatGetResourcesShouldImplementIPaginate()
        {
            var ignored = new[]
            {
                typeof (Sync.IRunbookProcessRepository).GetTypeInfo(),
                typeof (Sync.IDeploymentProcessRepository).GetTypeInfo(),
                typeof (Sync.IInterruptionRepository).GetTypeInfo(),
                typeof (Sync.IEventRepository).GetTypeInfo(),
                typeof (Sync.IVariableSetRepository).GetTypeInfo(),
                typeof (Sync.IChannelRepository).GetTypeInfo(),
                typeof (Sync.IProjectTriggerRepository).GetTypeInfo(),
                typeof (Sync.ICommunityActionTemplateRepository).GetTypeInfo(),
                typeof (Sync.IScopedUserRoleRepository).GetTypeInfo(),
                typeof (Sync.IUpgradeConfigurationRepository).GetTypeInfo()
            };

            var missing = SyncRepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.AsType().IsAssignableToGenericType(typeof(Sync.IGet<>)))
                .Except(RepositoryInterfaceTypes.Where(t => t.AsType().IsAssignableToGenericType(typeof(Sync.IPaginate<>))))
                .ToArray();

            if (missing.Any())
            {
                Assert.Fail($"Most repositories that get resources should implement IPaginate<TResource> unless the repository should target one specific resource like a singleton or child of another aggregate.{Environment.NewLine}{missing.Select(t => t.Name).NewLineSeparate()}");
            }
        }

        [Test]
        public void AsyncRepositoriesThatImplementCreateShouldAlsoImplementModify()
        {
            var ignored = new []
            {
                typeof(IDeploymentRepository).GetTypeInfo(),
                typeof(IRunbookRunRepository).GetTypeInfo(),
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
                Assert.Fail($"Repositories that implement ICreate<IResource> should usually implement IModify<IResource>.{Environment.NewLine}{missingModify.Select(t => t.Name).NewLineSeparate()}");
            }
        }

        [Test]
        public void SyncRepositoriesThatImplementCreateShouldAlsoImplementModify()
        {
            var ignored = new[]
            {
                typeof(Sync.IDeploymentRepository).GetTypeInfo(),
                typeof(Sync.IRunbookRunRepository).GetTypeInfo(),
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
                Assert.Fail($"Repositories that implement ICreate<IResource> should usually implement IModify<IResource>.{Environment.NewLine}{missingModify.Select(t => t.Name).NewLineSeparate()}");
            }
        }

        [Test]
        public void AsyncRepositoriesThatImplementCreateShouldAlsoImplementDelete()
        {
            var ignored = new[]
            {
                typeof(IDeploymentRepository).GetTypeInfo(),
                typeof(IRunbookRunRepository).GetTypeInfo(),
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
                Assert.Fail($"Repositories that implement ICreate<IResource> should usually implement IDelete<IResource>.{Environment.NewLine}{missingDelete.Select(t => t.Name).NewLineSeparate()}");
            }
        }

        [Test]
        public void SyncRepositoriesThatImplementCreateShouldAlsoImplementDelete()
        {
            var ignored = new[]
            {
                typeof(Sync.IDeploymentRepository).GetTypeInfo(),
                typeof(Sync.IRunbookRunRepository).GetTypeInfo(),
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
                Assert.Fail($"Repositories that implement ICreate<IResource> should usually implement IDelete<IResource>.{Environment.NewLine}{missingDelete.Select(t => t.Name).NewLineSeparate()}");
            }
        }

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
                .Except(new[] { typeof(LifecycleResource), typeof(RunbookProcessResource), typeof(DeploymentProcessResource), typeof(CertificateResource) })
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
    }
}
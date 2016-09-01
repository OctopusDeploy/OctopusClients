using System;
using System.Linq;
using System.Reflection;
using Conventional;
using Conventional.Conventions;
using NUnit.Framework;
using Octopus.Client.Extensions;
using Octopus.Client.Model;
using Octopus.Client.Repositories;
using Autofac;
using Autofac.Util;
using Nancy.Extensions;

namespace Octopus.Client.Tests.Conventions
{
    [TestFixture]
    public class ClientConventions
    {
        private static readonly Type[] ExportedTypes = typeof(IOctopusClient).Assembly.GetExportedTypes().Select(t => t).ToArray();
        private static readonly Type[] RepositoryInterfaceTypes = ExportedTypes
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .Where(t => t != typeof(IOctopusRepository))
            .ToArray();
        private static readonly Type[] RepositoryTypes = ExportedTypes
            .Where(t => !t.IsInterface && t.Name.EndsWith("Repository"))
            .Where(t => t != typeof(OctopusRepository))
            .ToArray();
        private static readonly Type[] ResourceTypes = ExportedTypes
            .Where(t => t.Name.EndsWith("Resource"))
            .ToArray();
        private static readonly Type[] RepositoryResourceTypes = ResourceTypes
            .Where(res => RepositoryTypes
                .Any(rep => rep.BaseType?.IsGenericType == true && rep.BaseType?.GetGenericArguments().Contains(res) == true))
            .ToArray();

        [Test]
        public void AllRepositoriesShouldBeAvailableViaIOctopusRepository()
        {
            var exposedTypes = typeof(IOctopusRepository).GetProperties()
                .Select(p => p.PropertyType)
                .ToArray();

            var missingTypes = RepositoryInterfaceTypes.Except(exposedTypes).ToArray();
            if (missingTypes.Any())
            {
                Assert.Fail($"All *Repository types should be exposed by {nameof(IOctopusRepository)}. Missing: {missingTypes.Select(t => t.Name)}");
            }
        }

        [Test]
        public void AllRepositoriesShouldBeAvailableViaOctopusRepository()
        {
            var exposedTypes = typeof(OctopusRepository).GetProperties()
                .Select(p => p.PropertyType)
                .ToArray();

            var missingTypes = RepositoryInterfaceTypes.Except(exposedTypes).ToArray();
            if (missingTypes.Any())
            {
                Assert.Fail($"All *Repository types should be exposed by {nameof(OctopusRepository)}. Missing: {missingTypes.Select(t => t.Name).CommaSeperate()}");
            }
        }

        [Test]
        public void AllRepositoriesShouldImplementNonGenericSimpleInterface()
        {
            var repositoryInterfaceMap = RepositoryTypes.Select(r => new { Repository = r, Interface = r.GetInterfaces().Where(i => !i.IsGenericType) }).ToArray();
            var missingInterface = repositoryInterfaceMap.Where(x => x.Interface == null).ToArray();

            if (missingInterface.Any())
            {
                Assert.Fail($"All *Repository types should implement a non-generic interface representing the repository contract {nameof(OctopusRepository)}.{Environment.NewLine}{missingInterface.Select(x => $"{x.Repository.Name} expected to implement I{x.Repository.Name}").NewLineSeperate()}");
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
                    .Where(m => m.DeclaringType == r)
                    .Select(m => new
                    {
                        DelcaredMethod = m,
                        r.GetInterfaces().Select(r.GetInterfaceMap).FirstOrDefault(map => map.TargetMethods.Contains(m)).InterfaceType
                    }).ToArray(),
                }).ToArray();

            var missingExposure = exposureMap.Where(x => x.DeclaredMethodMap.Any(map => map.InterfaceType == null)).ToArray();
            if (missingExposure.Any())
            {
                Assert.Fail($"The following repositories do not expose at least one of their public members by interface, and hence won't be accessible by clients.{Environment.NewLine}{exposureMap.Where(x => x.DeclaredMethodMap.Any(map => map.InterfaceType == null)).Select(x => $"{x.Repository.Name}: {x.DeclaredMethodMap.Where(map => map.InterfaceType == null).Select(map => $"{map.DelcaredMethod.Name}({map.DelcaredMethod.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}").CommaSeperate()})").CommaSeperate()}").NewLineSeperate()}");
            }
        }

        [Test]
        public void AllRepositoryInterfacesShouldFollowTheseConventions()
        {
            RepositoryInterfaceTypes
                .MustConformTo(Convention.MustLiveInNamespace("Octopus.Client.Repositories"))
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
                .Except(new[] { typeof(LifecycleResource), typeof(DeploymentProcessResource) })
                .MustConformTo(Convention.PropertiesMustHavePublicSetters)
                .WithFailureAssertion(Assert.Fail);
        }

        [Test]
        public void RepositoriesShouldNotRepresentMultipleResourceTypes()
        {
            var repositoryResourceMap = RepositoryTypes
                .Select(t => new
                {
                    Repository = t,
                    ResourceTypes = t.GetInterfaces().Concat(new[] { t.BaseType })
                        .Where(i => i.IsGenericType)
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
                        .Where(i => i.IsGenericType)
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
                typeof (DeploymentResource),
                typeof (TaskResource)
            };

            var shouldProbablyBeINamedResource = RepositoryResourceTypes
                .Except(ignored)
                .Where(t => t.GetProperty("Name") != null && !t.IsAssignableTo<INamedResource>())
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
        public void RepositoriesThatGetNamedResourcesShouldUsuallyImplementIFindByName()
        {
            var ignored = new[]
            {
                typeof (IChannelRepository),
                typeof (IProjectTriggerRepository)
            };

            var getsResources = RepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.IsAssignableToGenericType(typeof(IGet<>)))
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

        [Test]
        public void MostRepositoriesThatGetResourcesShouldImplementIPaginate()
        {
            var ignored = new[]
            {
                typeof (IDeploymentProcessRepository),
                typeof (IInterruptionRepository),
                typeof (IEventRepository),
                typeof (IVariableSetRepository),
                typeof (IChannelRepository),
                typeof (IProjectTriggerRepository)
            };

            var missing = RepositoryInterfaceTypes
                .Except(ignored)
                .Where(t => t.IsAssignableToGenericType(typeof(IGet<>)))
                .Except(RepositoryInterfaceTypes.Where(t => t.IsAssignableToGenericType(typeof(IPaginate<>))))
                .ToArray();

            if (missing.Any())
            {
                Assert.Fail($"Most repositories that get resources should implement IPaginate<TResource> unless the repository should target one specific resource like a singleton or child of another aggregate.{Environment.NewLine}{missing.Select(t => t.Name).NewLineSeperate()}");
            }
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
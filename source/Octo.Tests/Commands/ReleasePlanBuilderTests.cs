using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.Releases;
using Octopus.Cli.Model;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;
using Serilog;

namespace Octo.Tests.Commands
{
    [TestFixture]
    public class ReleasePlanBuilderTests
    {
        ReleasePlanBuilder builder;
        private ILogger logger;
        private IPackageVersionResolver versionResolver;
        private IChannelVersionRuleTester versionRuleTester;
        private IOctopusAsyncRepository repository;
        private IDeploymentProcessRepository deploymentProcessRepository;
        private IReleaseRepository releaseRepository;
        private IFeedRepository feedRepository;
        private ICommandOutputProvider commandOutputProvider;
        
        private ProjectResource projectResource;
        private ChannelResource channelResource;
        private string preReleaseTag;

        private DeploymentProcessResource deploymentProcessResource;
        private ReleaseTemplateResource releaseTemplateResource;
        private List<ChannelVersionRuleResource> channelVersionRules;
        private ChannelVersionRuleTestResult channelVersionRuleTestResult = new ChannelVersionRuleTestResult();
        private FeedResource feedResource;
        private List<PackageResource> packages = new List<PackageResource>();

        [SetUp]
        public void Setup()
        {
            // setup data objects
            channelVersionRules = new List<ChannelVersionRuleResource>();
            projectResource = new ProjectResource
            {
                DeploymentProcessId = TestHelpers.GetId("deploymentprocess"),
                Id = TestHelpers.GetId("project")
            };
            deploymentProcessResource = new DeploymentProcessResource
            {
                ProjectId = projectResource.Id,
                Id = projectResource.DeploymentProcessId
            };

            releaseTemplateResource = new ReleaseTemplateResource
            {
                DeploymentProcessId = projectResource.DeploymentProcessId,
                Packages = new List<ReleaseTemplatePackage>(),
                Id = TestHelpers.GetId("releaseTemplate")
            };
            channelResource = new ChannelResource
            {
                IsDefault = true,
                Id = TestHelpers.GetId("channel"),
                ProjectId = projectResource.Id,
                Rules = channelVersionRules,
                Name = TestHelpers.GetId("channelname")
            };
            feedResource = new FeedResource
            {
                Links = new LinkCollection {{"SearchTemplate", TestHelpers.GetId("searchUri")}}
            };
            
            // setup mocks
            logger = Substitute.For<ILogger>();
            versionResolver = Substitute.For<IPackageVersionResolver>();
            versionRuleTester = Substitute.For<IChannelVersionRuleTester>();
            commandOutputProvider = Substitute.For<ICommandOutputProvider>();

            deploymentProcessRepository = Substitute.For<IDeploymentProcessRepository>();
            deploymentProcessRepository.Get(projectResource.DeploymentProcessId)
                .Returns(Task.FromResult(deploymentProcessResource));
            deploymentProcessRepository
                .GetTemplate(Arg.Is<DeploymentProcessResource>(deploymentProcessResource),
                    Arg.Is<ChannelResource>(channelResource)).Returns(Task.FromResult(releaseTemplateResource));
            versionRuleTester
                .Test(Arg.Any<IOctopusAsyncRepository>(), Arg.Any<ChannelVersionRuleResource>(), Arg.Any<string>())
                .Returns(Task.FromResult(channelVersionRuleTestResult));

            releaseRepository = Substitute.For<IReleaseRepository>();
            feedRepository = Substitute.For<IFeedRepository>();
            feedRepository.Get(Arg.Any<string>()).Returns(feedResource);

            repository = Substitute.For<IOctopusAsyncRepository>();
            repository.DeploymentProcesses.Returns(deploymentProcessRepository);
            repository.Releases.Returns(releaseRepository);
            repository.Feeds.Returns(feedRepository);
            repository.Client
                .Get<List<PackageResource>>(Arg.Any<string>(), Arg.Any<IDictionary<string, object>>()).Returns(packages);

            builder = new ReleasePlanBuilder(logger, versionResolver, versionRuleTester, commandOutputProvider);
        }

        [Test]
        public void NoPackageOrScriptSteps_ShouldNotBeViable()
        {
            // act
            var plan = ExecuteBuild();

            // assert
            plan.Should().NotBeNull();
            plan.IsViableReleasePlan().Should().BeFalse();
        }

        [Test]
        public void DisabledScriptSteps_ShouldNotBeAViablePlan()
        {
            // arrange
            var deploymentStepResource = ResourceBuilderHelpers.GetStep();
            deploymentStepResource.Actions.Add(ResourceBuilderHelpers.GetAction().Disabled().WithChannel(channelResource.Id));
            deploymentProcessResource.Steps.Add(deploymentStepResource);

            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeFalse();
        }

        [Test]
        public void SingleEnabledScriptStep_ShouldBeAViablePlan()
        {
            // arrange
            var deploymentStepResource = ResourceBuilderHelpers.GetStep();
            deploymentStepResource.Actions.Add(ResourceBuilderHelpers.GetAction().WithChannel(channelResource.Id));
            deploymentProcessResource.Steps.Add(deploymentStepResource);

            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeTrue();
        }

        [Test]
        public void SingleEnabledScriptStepScopedToDifferentChannel_ShouldNotBeAViablePlan()
        {
            // arrange
            var deploymentStepResource = ResourceBuilderHelpers.GetStep();
            deploymentStepResource.Actions.Add(ResourceBuilderHelpers.GetAction().WithChannel(TestHelpers.GetId("differentchannel")));
            deploymentProcessResource.Steps.Add(deploymentStepResource);

            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeFalse();
        }

        [Test]
        public void SingleEnabledScriptStepScopedToMatchingChannel_ShouldBeViablePlan()
        {
            // arrange
            var deploymentStepResource = ResourceBuilderHelpers.GetStep();
            deploymentStepResource.Actions.Add(ResourceBuilderHelpers.GetAction().WithChannel(channelResource.Id));
            deploymentProcessResource.Steps.Add(deploymentStepResource);

            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeTrue();
        }

        [Test]
        public void SinglePackageStep_ShouldBeViablePlan()
        {
            // arrange
            var deploymentStepResource = ResourceBuilderHelpers.GetStep();
            deploymentStepResource.Actions.Add(ResourceBuilderHelpers.GetAction().WithChannel(channelResource.Id).WithPackage());
            deploymentProcessResource.Steps.Add(deploymentStepResource);

            releaseTemplateResource.Packages.Add(GetReleaseTemplatePackage().WithPackage().WithVersion("1.0.0", versionResolver));
            channelVersionRuleTestResult.IsSatified();


            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeTrue();
        }

        [Test]
        public void StepWithNoPackageVersion_ShouldNotBeViablePlan()
        {
            // arrange 
            releaseTemplateResource.Packages.Add(GetReleaseTemplatePackage().WithPackage()
                .WithVersion(string.Empty, versionResolver));

            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeFalse();
        }

        [Test]
        public void StepFlaggedAsUnresolvable_ShouldNotBeViablePlan()
        {
            // arrange
            releaseTemplateResource.Packages.Add(GetReleaseTemplatePackage().WithPackage().IsNotResolvable());

            // act
            var plan = ExecuteBuild();
            
            // assert
            plan.IsViableReleasePlan().Should().BeFalse();
        }

        [Test]
        public void MultipleSteps_OneNotResolvable_ShouldNotBeViablePlan()
        {
            // arrange
            var releaseTemplatePackage = GetReleaseTemplatePackage().WithPackage();
            releaseTemplatePackage.IsResolvable = false;
            releaseTemplateResource.Packages.Add(releaseTemplatePackage);

            releaseTemplatePackage = GetReleaseTemplatePackage().WithPackage();
            releaseTemplatePackage.IsResolvable = true;
            releaseTemplateResource.Packages.Add(releaseTemplatePackage);

            repository.Client.Get<IList<PackageResource>>(Arg.Any<string>(), Arg.Any<IDictionary<string, object>>())
                .Returns(new List<PackageResource> {new PackageResource {Version = "1.0.0"}});

            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeFalse();
        }

        [Test]
        public void SingleStep_ResolvableUsingFeed_ShouldBeViablePlan()
        {
            // arrange
            releaseTemplateResource.Packages.Add(GetReleaseTemplatePackage().WithPackage().IsResolvable());
            packages.Add(new PackageResource { Version = "1.0.0"});

            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeTrue();
        }

        [Test]
        public void ChannelVersionRuleForNamedPackageReference_ShouldBeUsedToFilterPackages()
        {
            // arrange
            var deploymentStepResource = ResourceBuilderHelpers.GetStep();
            var action = ResourceBuilderHelpers.GetAction();
            action.Packages.Add(new PackageReference("Acme", "Acme", "feeds-builtin", PackageAcquisitionLocation.Server));
            deploymentStepResource.Actions.Add(action);
            deploymentProcessResource.Steps.Add(deploymentStepResource);
            channelResource.AddRule(new ChannelVersionRuleResource
            {
                ActionPackages = new List<DeploymentActionPackageResource>
                {
                    new DeploymentActionPackageResource(action.Name, "Acme")
                },
                VersionRange = "(,1.0)"
            });
            
            packages.Add(new PackageResource { Version = "1.0.1"});

            releaseTemplateResource.Packages.Add(new ReleaseTemplatePackage{ActionName = action.Name, PackageReferenceName = "Acme", IsResolvable = true});
            channelVersionRuleTestResult.IsSatified();
            
            repository.Client
                .Get<List<PackageResource>>(Arg.Any<string>(), Arg.Is<IDictionary<string, object>>(d => d.ContainsKey("versionRange") && (string)d["versionRange"] == "(,1.0)")).Returns(new List<PackageResource>());
            
            // act
            var plan = ExecuteBuild();

            // assert
            plan.IsViableReleasePlan().Should().BeFalse();
        }

        private static ReleaseTemplatePackage GetReleaseTemplatePackage()
        {
            return new ReleaseTemplatePackage
            {
                ActionName = TestHelpers.GetId("step")
            };
        }

        private ReleasePlan ExecuteBuild()
        {
            var task = ExecuteBuildAsync();
            return task.Result;
        }

        private async Task<ReleasePlan> ExecuteBuildAsync()
        {
            return await builder.Build(repository, projectResource, channelResource, preReleaseTag);
        }
    }

    public static class ResourceBuilderHelpers
    {
        public static DeploymentActionResource WithPackage(this DeploymentActionResource action)
        {
            action.Properties["Octopus.Action.Package.PackageId"] = TestHelpers.GetId("package");
            return action;
        }

        public static DeploymentActionResource Disabled(this DeploymentActionResource action)
        {
            action.IsDisabled = true;
            return action;
        }

        public static DeploymentActionResource WithChannel(this DeploymentActionResource action, string channelId)
        {
            action.Channels.Add(channelId);
            return action;
        }

        public static ReleaseTemplatePackage WithPackage(this ReleaseTemplatePackage releaseTemplatePackage)
        {
            releaseTemplatePackage.PackageId = TestHelpers.GetId("package");
            releaseTemplatePackage.FeedId = TestHelpers.GetId("feed");
            return releaseTemplatePackage;
        }

        public static ReleaseTemplatePackage IsResolvable(this ReleaseTemplatePackage releaseTemplatePackage)
        {
            releaseTemplatePackage.IsResolvable = true;
            return releaseTemplatePackage;
        }

        public static ReleaseTemplatePackage IsNotResolvable(this ReleaseTemplatePackage releaseTemplatePackage)
        {
            releaseTemplatePackage.IsResolvable = false;
            return releaseTemplatePackage;
        }

        public static ReleaseTemplatePackage WithVersion(this ReleaseTemplatePackage releaseTemplatePackage,
            string version, IPackageVersionResolver versionResolver)
        {
            versionResolver.ResolveVersion(releaseTemplatePackage.ActionName, releaseTemplatePackage.PackageId)
                .Returns(version);
            versionResolver.ResolveVersion(releaseTemplatePackage.ActionName, releaseTemplatePackage.PackageId, null)
                .Returns(version);
            return releaseTemplatePackage;
        }

        public static ChannelVersionRuleTestResult IsSatified(this ChannelVersionRuleTestResult versionRuleTestResult)
        {
            versionRuleTestResult.SatisfiesPreReleaseTag = true;
            versionRuleTestResult.SatisfiesVersionRange = true;
            return versionRuleTestResult;
        }

        public static DeploymentStepResource GetStep()
        {
            return new DeploymentStepResource
            {
                Id = TestHelpers.GetId("deploymentstep"),
                Name = TestHelpers.GetId("deploymentstepName"),
            };
        }

        public static DeploymentActionResource GetAction()
        {
            return new DeploymentActionResource
            {
                IsDisabled = false,
                Id = TestHelpers.GetId("action"),
                Name = TestHelpers.GetId("actionname")
            };
        }

    }

    public static class TestHelpers
    {
        public static string GetId()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        public static string GetId(string prefix)
        {
            return $"{prefix}-{GetId()}";
        }

    }
}

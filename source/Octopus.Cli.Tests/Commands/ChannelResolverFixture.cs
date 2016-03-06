using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Cli.Commands;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Infrastructure;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ChannelResolverFixture
    {
        ChannelResolver resolver;
        IChannelResolverHelper helper;
        IPackageVersionResolver versionResolverMock;
        IOctopusRepository repositoryMock;

        [SetUp]
        public void SetUp()
        {
            helper = Substitute.For<IChannelResolverHelper>();
            versionResolverMock = Substitute.For<IPackageVersionResolver>();
            repositoryMock = Substitute.For<IOctopusRepository>();

            resolver = new ChannelResolver(Logger.Default, helper, versionResolverMock);
        }

        [Test]
        public void ResolveByNameShouldSucceed()
        {
            var channels = new List<ChannelResource>()
            {
                new ChannelResource { Name = "Channel1" },
                new ChannelResource { Name = "Channel2" }
            };
            resolver.RegisterChannels(channels);

            Assert.That(resolver.ResolveByName("Channel1").Name, Is.EqualTo("Channel1"));
            Assert.That(resolver.ResolveByName("Channel2").Name, Is.EqualTo("Channel2"));
        }

        [Test]
        public void ResolveByNameShouldBeCaseInsensitive()
        {
            var channels = new List<ChannelResource>()
            {
                new ChannelResource { Name = "Channel1" }
            };
            resolver.RegisterChannels(channels);

            Assert.That(resolver.ResolveByName("channel1").Name, Is.EqualTo("Channel1"));
            Assert.That(resolver.ResolveByName("ChAnnEL1").Name, Is.EqualTo("Channel1"));
            Assert.That(resolver.ResolveByName("CHANNEL1").Name, Is.EqualTo("Channel1"));
        }

        [Test]
        public void ResolveByNameShouldThrowWhenNotFound()
        {
            var channels = new List<ChannelResource>()
            {
                new ChannelResource { Name = "Channel1" },
                new ChannelResource { Name = "Channel2" }
            };
            resolver.RegisterChannels(channels);

            Assert.Throws<CouldNotFindException>(() => resolver.ResolveByName("NotAChannel"));
        }

        public void SetupDefaultAutoChannelData()
        {
            var channel1 = new ChannelResource
            {
                Name = "Channel1",
                Rules = new List<ChannelVersionRuleResource>
                {
                    new ChannelVersionRuleResource
                    {
                        Id = "rule1",
                        Actions = new ReferenceCollection(new[] { "step1", "step2" })
                    },
                    new ChannelVersionRuleResource
                    {
                        Id = "rule2",
                        Actions = new ReferenceCollection(new[] { "step3" })
                    }
                }
            };

            var channel2 = new ChannelResource
            {
                Name = "Channel2",
                Rules = new List<ChannelVersionRuleResource>
                {
                    new ChannelVersionRuleResource
                    {
                        Id = "rule3",
                        Actions = new ReferenceCollection(new[] { "step1", "step2" })
                    },
                    new ChannelVersionRuleResource
                    {
                        Id = "rule4",
                        Actions = new ReferenceCollection(new[] { "step3" })
                    }
                }
            };

            var channels = new ChannelResource[] { channel1, channel2 };
            resolver.RegisterChannels(channels);

            // Default version for steps/packages
            versionResolverMock.ResolveVersion(null).ReturnsForAnyArgs("1.0.0");

            // Set expected step count to however many steps channel1 has
            helper.GetApplicableStepCount(null, null, null, null).ReturnsForAnyArgs(channel1.Rules.Sum(r => r.Actions.Count));
        }

        [Test]
        public void ResolveByRuleShouldSucceedWhenOneChannelMatches()
        {
            SetupDefaultAutoChannelData();

            // Setup Channel1 to pass match
            helper
                .TestChannelRuleAgainstOctopusApi(
                    Arg.Any<IOctopusRepository>(),
                    Arg.Is<ChannelResource>(c => c.Name == "Channel1"),
                    Arg.Any<ChannelVersionRuleResource>(),
                    Arg.Any<string>())
                .Returns(true);

            var channel = resolver.ResolveByRules(repositoryMock, versionResolverMock);
            Assert.That(channel, Is.Not.Null);
            Assert.That(channel.Name, Is.EqualTo("Channel1"));
        }

        [Test]
        public void ResolveByRuleShouldSucceedWhenAllRulesMatch()
        {
            SetupDefaultAutoChannelData();

            // Setup rule 1 and 2 to pass
            helper
                .TestChannelRuleAgainstOctopusApi(
                    Arg.Any<IOctopusRepository>(),
                    Arg.Any<ChannelResource>(),
                    Arg.Is<ChannelVersionRuleResource>(r => r.Id == "rule1" || r.Id == "rule2"),
                    Arg.Any<string>())
                .Returns(true);

            var channel = resolver.ResolveByRules(repositoryMock, versionResolverMock);
            Assert.That(channel, Is.Not.Null);
            Assert.That(channel.Name, Is.EqualTo("Channel1"));
        }

        [Test]
        public void ResolveByRuleShouldNotSelectChannelWhenItHasNoRules()
        {
            SetupDefaultAutoChannelData();

            // Override channel to one with no rules
            resolver.RegisterChannels(new ChannelResource[] { new ChannelResource { Name = "Channel1", Rules = new List<ChannelVersionRuleResource>() } });

            // Setup all channels to pass
            helper
                .TestChannelRuleAgainstOctopusApi(null, null, null, null)
                .ReturnsForAnyArgs(true);

            // No matches should occur, as channel with no rules doesn't get to the test phase
            var ex = Assert.Throws<Exception>(() => resolver.ResolveByRules(repositoryMock, versionResolverMock));
            Assert.IsTrue(ex.Message.Contains("no channels were matched"));
        }

        [Test]
        public void ResolveByRuleShouldNotSelectChannelWhenItDoesntHaveRulesForAllRequiredSteps()
        {
            SetupDefaultAutoChannelData();

            // Override channel to one that only covers 2 out of 3 steps
            resolver.RegisterChannels(
                new ChannelResource[]
                {
                    new ChannelResource
                    {
                        Name = "Channel1",
                        Rules = new List<ChannelVersionRuleResource>()
                        {
                            new ChannelVersionRuleResource { Actions = new ReferenceCollection { "step1", "step2" } }
                        }
                    }
                });

            // Setup all channels to pass
            helper
                .TestChannelRuleAgainstOctopusApi(null, null, null, null)
                .ReturnsForAnyArgs(true);

            // No matches should occur, as channel with some steps not covered by rules doesn't get to the test phase
            var ex = Assert.Throws<Exception>(() => resolver.ResolveByRules(repositoryMock, versionResolverMock));
            Assert.IsTrue(ex.Message.Contains("no channels were matched"));
        }

        [Test]
        public void ResolveByRuleShouldThrowWhenNoChannelsMatch()
        {
            SetupDefaultAutoChannelData();

            // Setup all channels to fail match
            helper
                .TestChannelRuleAgainstOctopusApi(null, null, null, null)
                .ReturnsForAnyArgs(false);

            var ex = Assert.Throws<Exception>(() => resolver.ResolveByRules(repositoryMock, versionResolverMock));
            Assert.IsTrue(ex.Message.Contains("no channels were matched"));
        }

        [Test]
        public void ResolveByRuleShouldThrowWhenMultipleChannelsMatch()
        {
            SetupDefaultAutoChannelData();

            // Setup all channels to pass match
            helper
                .TestChannelRuleAgainstOctopusApi(null, null, null, null)
                .ReturnsForAnyArgs(true);

            var ex = Assert.Throws<Exception>(() => resolver.ResolveByRules(repositoryMock, versionResolverMock));
            Assert.IsTrue(ex.Message.Contains("matched all steps"));
        }
    }
}

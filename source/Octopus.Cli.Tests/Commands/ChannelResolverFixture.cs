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
        IChannelResolverHelper helperMock;

        [SetUp]
        public void SetUp()
        {
            helperMock = Substitute.For<IChannelResolverHelper>();
            
            resolver = new ChannelResolver(Logger.Default, helperMock);
        }

        [Test]
        public void ResolveByNameShouldSucceed()
        {
            var channels = new List<ChannelResource>()
            {
                new ChannelResource { Name = "Channel1" },
                new ChannelResource { Name = "Channel2" }
            };
            helperMock.GetChannels()
                .ReturnsForAnyArgs(channels);

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
            helperMock.GetChannels()
                .ReturnsForAnyArgs(channels);

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
            helperMock.GetChannels()
                .ReturnsForAnyArgs(channels);

            Assert.Throws<CouldNotFindException>(() => resolver.ResolveByName("NotAChannel"));
        }

        List<ChannelResource> defaultChannels;
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

            defaultChannels = new List<ChannelResource>() { channel1, channel2 };
            helperMock.GetChannels()
                .ReturnsForAnyArgs(defaultChannels);

            // Default version for steps/packages
            helperMock.ResolveVersion(null, null).ReturnsForAnyArgs("1.0.0");
            
            // Set expected step count to however many steps channel1 has
            helperMock.GetApplicableStepCount(null).ReturnsForAnyArgs(channel1.Rules.Sum(r => r.Actions.Count));
        }

        [Test]
        public void ResolveByRuleShouldSucceedWhenOneChannelMatches()
        {
            SetupDefaultAutoChannelData();

            // Setup Channel1 to pass match
            helperMock
                .TestChannelRuleAgainstOctopusApi(
                    Arg.Is<ChannelResource>(c => c.Name == "Channel1"),
                    Arg.Any<ChannelVersionRuleResource>(),
                    Arg.Any<string>())
                .Returns(true);

            var channel = resolver.ResolveByRules();
            Assert.That(channel, Is.Not.Null);
            Assert.That(channel.Name, Is.EqualTo("Channel1"));
        }

        [Test]
        public void ResolveByRuleShouldSucceedWhenAllRulesMatch()
        {
            SetupDefaultAutoChannelData();

            // Setup rule 1 and 2 to pass
            helperMock
                .TestChannelRuleAgainstOctopusApi(
                    Arg.Any<ChannelResource>(),
                    Arg.Is<ChannelVersionRuleResource>(r => r.Id == "rule1" || r.Id == "rule2"),
                    Arg.Any<string>())
                .Returns(true);

            var channel = resolver.ResolveByRules();
            Assert.That(channel, Is.Not.Null);
            Assert.That(channel.Name, Is.EqualTo("Channel1"));
        }

        [Test]
        public void ResolveByRuleShouldNotSelectChannelWhenItHasNoRules()
        {
            SetupDefaultAutoChannelData();

            // Override channel to one with no rules
            helperMock.GetChannels()
                .ReturnsForAnyArgs(new ChannelResource[] { new ChannelResource { Name = "Channel1", Rules = new List<ChannelVersionRuleResource>() } });

            // Setup all channels to pass
            helperMock
                .TestChannelRuleAgainstOctopusApi(null, null, null)
                .ReturnsForAnyArgs(true);

            // No matches should occur, as channel with no rules doesn't get to the test phase
            var ex = Assert.Throws<Exception>(() => resolver.ResolveByRules());
            Assert.IsTrue(ex.Message.Contains("no channels were matched"));
        }

        [Test]
        public void ResolveByRuleShouldNotSelectChannelWhenItDoesntHaveRulesForAllRequiredSteps()
        {
            SetupDefaultAutoChannelData();

            // Override channel to one that only covers 2 out of 3 steps
            helperMock.GetChannels()
                .ReturnsForAnyArgs(
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
            helperMock
                .TestChannelRuleAgainstOctopusApi(null, null, null)
                .ReturnsForAnyArgs(true);

            // No matches should occur, as channel with some steps not covered by rules doesn't get to the test phase
            var ex = Assert.Throws<Exception>(() => resolver.ResolveByRules());
            Assert.IsTrue(ex.Message.Contains("no channels were matched"));
        }

        [Test]
        public void ResolveByRuleShouldThrowWhenNoChannelsMatchAndThereIsNoDefault()
        {
            SetupDefaultAutoChannelData();

            // Setup all channels to fail match
            helperMock
                .TestChannelRuleAgainstOctopusApi(null, null, null)
                .ReturnsForAnyArgs(false);

            // Ensure exception occured (as no channels were marked default)
            var ex = Assert.Throws<Exception>(() => resolver.ResolveByRules());
            Assert.IsTrue(ex.Message.Contains("no channels were matched"));
        }

        [Test]
        public void ResolveByRuleShouldSelectDefaultWhenNoChannelsMatch()
        {
            SetupDefaultAutoChannelData();

            // Add a default channel
            var channels = defaultChannels;
            channels.Add(new ChannelResource { Name = "Channel3", IsDefault = true, Rules = new List<ChannelVersionRuleResource>() });
            helperMock.GetChannels()
                .ReturnsForAnyArgs(channels);

            // Setup all channels to fail match
            helperMock
                .TestChannelRuleAgainstOctopusApi(null, null, null)
                .ReturnsForAnyArgs(false);

            // Ensure default channel was selected
            var channel = resolver.ResolveByRules();
            Assert.That(channel, Is.Not.Null);
            Assert.That(channel.Name, Is.EqualTo("Channel3"));
        }

        [Test]
        public void ResolveByRuleShouldThrowWhenMultipleChannelsMatch()
        {
            SetupDefaultAutoChannelData();

            // Setup all channels to pass match
            helperMock
                .TestChannelRuleAgainstOctopusApi(null, null, null)
                .ReturnsForAnyArgs(true);

            var ex = Assert.Throws<Exception>(() => resolver.ResolveByRules());
            Assert.IsTrue(ex.Message.Contains("matched all steps"));
        }
    }
}

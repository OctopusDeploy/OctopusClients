using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class EventNotificationSubscription
    {
        public EventNotificationSubscription()
        {
            this.Filter = new EventNotificationSubscriptionFilter();
            this.EmailTeams = new ReferenceCollection();
            this.WebhookTeams = new ReferenceCollection();
            this.SlackChannelIds = new ReferenceCollection();
            this.SlackChannelNames = new ReferenceCollection();
            this.TeamsChannels = new List<TeamsChannelSubscriptionTarget>();
        }

        public EventNotificationSubscriptionFilter Filter { get; set; }


        public ReferenceCollection EmailTeams { get; set; }

        public TimeSpan EmailFrequencyPeriod { get; set; }

        public EmailPriority EmailPriority { get; set; }

        public DateTimeOffset? EmailDigestLastProcessed { get; set; }

        public long? EmailDigestLastProcessedEventAutoId { get; set; }

        public string EmailShowDatesInTimeZoneId { get; set; }


        public string WebhookURI { get; set; }

        public ReferenceCollection WebhookTeams { get; set; }

        public TimeSpan WebhookTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public string WebhookHeaderKey { get; set; }
        public PropertyValueResource WebhookHeaderValue { get; set; }

        public DateTimeOffset? WebhookLastProcessed { get; set; }

        public long? WebhookLastProcessedEventAutoId { get; set; }




        public ReferenceCollection SlackChannelIds { get; set; }

        public ReferenceCollection SlackChannelNames { get; set; }

        public TimeSpan SlackFrequencyPeriod { get; set; }

        public SlackDigestFormat SlackDigestFormat { get; set; }

        public DateTimeOffset? SlackDigestLastProcessed { get; set; }

        public long? SlackDigestLastProcessedEventAutoId { get; set; }




        public List<TeamsChannelSubscriptionTarget> TeamsChannels { get; set; }

        public TimeSpan TeamsFrequencyPeriod { get; set; }

        public DateTimeOffset? TeamsDigestLastProcessed { get; set; }

        public long? TeamsDigestLastProcessedEventAutoId { get; set; }
    }

    public class TeamsChannelSubscriptionTarget
    {
        // Client-generated and stable: it identifies which stored WebhookUrl an unchanged HasValue refers to.
        public string Id { get; set; }

        public string Name { get; set; }

        public PropertyValueResource WebhookUrl { get; set; }
    }

    public class EventNotificationSubscriptionFilter
    {
        public EventNotificationSubscriptionFilter()
        {
            this.Users = new List<string>();
            this.Projects = new List<string>();
            this.ProjectGroups = new List<string>();
            this.Environments = new List<string>();
            this.EventGroups = new List<string>();
            this.EventCategories = new List<string>();
            this.EventAgents = new List<string>();
            this.Tenants = new List<string>();
            this.Tags = new List<string>();
            this.DocumentTypes = new List<string>();
        }

        public IList<string> Users { get; set; }
        public IList<string> Projects { get; set; }
        public IList<string> ProjectGroups { get; set; }
        public IList<string> Environments { get; set; }
        public IList<string> EventGroups { get; set; }
        public IList<string> EventCategories { get; set; }
        public IList<string> EventAgents { get; set; }
        public IList<string> Tenants { get; set; }
        public IList<string> Tags { get; set; }
        public IList<string> DocumentTypes { get; set; }
    }
}

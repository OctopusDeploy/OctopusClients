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
        }

        public EventNotificationSubscriptionFilter Filter { get; set; }


        public ReferenceCollection EmailTeams { get; set; }

        public TimeSpan EmailFrequencyPeriod { get; set; }

        public DateTimeOffset? EmailDigestLastProcessed { get; set; }

        public long? EmailDigestLastProcessedEventAutoId { get; set; }

        public string EmailShowDatesInTimeZoneId { get; set; }


        public string WebhookURI { get; set; }

        public ReferenceCollection WebhookTeams { get; set; }

        public DateTimeOffset? WebhookLastProcessed { get; set; }

        public long? WebhookLastProcessedEventAutoId { get; set; }
    }

    public class EventNotificationSubscriptionFilter
    {
        public EventNotificationSubscriptionFilter()
        {
            this.Users = new List<string>();
            this.Projects = new List<string>();
            this.Environments = new List<string>();
            this.EventGroups = new List<string>();
            this.EventCategories = new List<string>();
            this.Tenants = new List<string>();
            this.Tags = new List<string>();
            this.DocumentTypes = new List<string>();
        }

        public IList<string> Users { get; set; }
        public IList<string> Projects { get; set; }
        public IList<string> Environments { get; set; }
        public IList<string> EventGroups { get; set; }
        public IList<string> EventCategories { get; set; }
        public IList<string> Tenants { get; set; }
        public IList<string> Tags { get; set; }
        public IList<string> DocumentTypes { get; set; }
    }
}

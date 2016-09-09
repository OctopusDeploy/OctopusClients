using System;

namespace Octopus.Client.Model
{
    public class ServerActivitiesResource : Resource
    {
        // Roots of supervision hierarchies; most of these will be
        // Task Controllers; also include activities supervised
        // by remote actors, e.g. file receivers controlled from
        // tentacles.
        public ServerActivityResource[] OrchestratedActivities { get; set; }
        // "Well known" actors- the dispatcher, clock, undeliverable
        // message queue, logger
        public ServerActivityResource[] SystemServices { get; set; }
        // Fragments of supervision hierarchies, where the supervisor is not known.
        public ServerActivityResource[] OrphanedActivities { get; set; }
    }
}
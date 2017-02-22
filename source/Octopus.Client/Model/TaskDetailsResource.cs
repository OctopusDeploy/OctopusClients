using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Octopus.Client.Model
{
    public class TaskDetailsResource : Resource
    {
        private ActivityElement activityLog;

        public TaskResource Task { get; set; }

        public IList<ActivityElement> ActivityLogs { get; set; }

        /// <summary>
        /// Returned by Pre 3.4 servers
        /// Sets the ActivityLogs property as well if it does not already have a value
        /// </summary>
        [Obsolete("Use ActivityLogs property")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ActivityElement ActivityLog
        {
            get { return activityLog; }
            set
            {
                activityLog = value;
                if (ActivityLogs == null && activityLog != null)
                    ActivityLogs = new List<ActivityElement>() {activityLog};
            }
        }

        public TaskProgress Progress { get; set; }
    }
}
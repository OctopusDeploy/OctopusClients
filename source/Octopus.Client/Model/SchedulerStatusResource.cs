namespace Octopus.Client.Model
{
    public class SchedulerStatusResource : Resource
    {
        public bool IsRunning { get; set; }
        public ScheduledTaskStatusResource[] TaskStatus { get; set; }
    }
}
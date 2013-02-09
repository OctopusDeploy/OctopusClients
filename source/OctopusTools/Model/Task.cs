using System;

namespace OctopusTools.Model
{
    public class Task : Resource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public string Output { get; set; }
        public DateTime QueueTime { get; set; }
        public string State { get; set; }
        public string Duration { get; set; }
        public bool IsFinished
        {
            get { return State == "Success" || State == "Failed" || State == "Cancelled" || State == "TimedOut"; }
        }

        public bool FinishedSuccessfully
        {
            get { return State == "Success"; }
        }
    }
}
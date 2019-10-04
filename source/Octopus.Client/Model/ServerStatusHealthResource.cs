namespace Octopus.Client.Model
{
    public class ServerStatusHealthResource : Resource
    {
        public bool IsOperatingNormally { get; set; }
        public string Description { get; set; }
        public bool IsEntireClusterReadOnly { get; set; }
        public bool IsEntireClusterDrainingTasks { get; set; }
        public bool IsCompliantWithLicense { get; set; }

        /// <summary>
        /// Null value means this instance does not support dynamic workers or does not have any pools
        /// </summary>
        public bool? IsDynamicWorkerPoolOperatingNormally { get; set; }
    }
}
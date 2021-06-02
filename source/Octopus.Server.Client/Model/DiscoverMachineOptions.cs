namespace Octopus.Client.Model
{
    public class DiscoverMachineOptions
    {
        public DiscoverMachineOptions(string host)
        {
            Host = host;
        }
        
        public string Host { get; }
        public int Port { get; set; } = 10933;
        public DiscoverableEndpointType? Type { get; set; }
        public ProxyResource Proxy { get; set; }
    }
}
namespace Octopus.Client.Model.Endpoints
{
    public class OfflineDropDestinationResource
    {
        public OfflineDropDestinationType DestinationType { get; set; }
        
        [Trim]
        public string DropFolderPath { get; set;}
    }
}
namespace Octopus.Client.Model.Endpoints
{
    public class OfflineDropDestinationResource
    {
        /// <summary>
        /// Constructs an offline-drop destination of type Artifact
        /// </summary>
        public OfflineDropDestinationResource()
        {
            DestinationType = OfflineDropDestinationType.Artifact;
        }
        
        /// <summary>
        /// Constructs an offline-drop destination of type FileSystem, with the drop-folder path set
        /// to the specified path.
        /// </summary>
        /// <param name="dropFolderPath"></param>
        public OfflineDropDestinationResource(string dropFolderPath)
        {
            DestinationType = OfflineDropDestinationType.FileSystem;
            DropFolderPath = dropFolderPath;
        }
        
        /// <summary>
        /// OfflineDrop targets can be written either as Octopus artifacts or to a file-system path
        /// </summary>
        public OfflineDropDestinationType DestinationType { get; set; }
        
        [Trim]
        public string DropFolderPath { get; set;}
    }
}
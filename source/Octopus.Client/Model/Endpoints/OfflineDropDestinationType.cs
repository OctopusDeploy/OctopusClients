namespace Octopus.Client.Model.Endpoints
{
    /// <summary>
    /// Offline-drop packages can be written either to a file-system directory or as an Octopus artifact
    /// </summary>
    public enum OfflineDropDestinationType
    {
        Artifact,
        FileSystem
    }
}
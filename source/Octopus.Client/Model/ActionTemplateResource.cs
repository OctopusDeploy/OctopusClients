namespace Octopus.Client.Model
{
    public class ActionTemplateResource : ActionTemplateVersionResource, INamedResource
    {
        public string CommunityActionTemplateId { get; set; }
    }
}
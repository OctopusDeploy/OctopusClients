using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ActionTemplateCategoryResource : IResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public LinkCollection Links { get; set; }
    }
}

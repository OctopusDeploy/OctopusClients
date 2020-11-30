namespace Octopus.Client.Model.VersionControl
{
    public class ConvertProjectToVersionControlledResponse : IHaveCustomerVisibleMessages
    {
        public ConvertProjectToVersionControlledResponse()
        {
            Messages = new MessageCollection();
        }

        public MessageCollection Messages { get; }
    }
}
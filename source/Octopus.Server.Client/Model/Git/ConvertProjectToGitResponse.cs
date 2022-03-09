namespace Octopus.Client.Model.Git
{
    public class ConvertProjectToGitResponse : IHaveCustomerVisibleMessages
    {
        public ConvertProjectToGitResponse()
        {
            Messages = new MessageCollection();
        }

        public MessageCollection Messages { get; }
    }
}
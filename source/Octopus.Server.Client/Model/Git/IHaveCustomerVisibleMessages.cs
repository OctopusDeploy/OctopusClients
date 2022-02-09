namespace Octopus.Client.Model.Git
{
    public interface IHaveCustomerVisibleMessages
    {
        MessageCollection Messages { get; }
    }
}
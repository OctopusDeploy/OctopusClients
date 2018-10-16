using System.Threading.Tasks;

namespace Octopus.Client.Declarative
{
    public interface IDeclarativeResource
    {
        Task Apply(IOctopusAsyncRepository repository, IApplyContext context);
    }
}
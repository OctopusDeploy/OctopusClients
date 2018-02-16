using System.Threading.Tasks;

namespace Octopus.Cli.Importers
{
    public interface IImporter
    {
        Task<bool> Validate(params string[] parameters);
        Task Import(params string[] parameters);
    }
}
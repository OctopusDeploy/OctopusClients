using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ILibraryVariableSetRepository :
        ICreate<LibraryVariableSetResource>,
        IGet<LibraryVariableSetResource>,
        IModify<LibraryVariableSetResource>,
        IDelete<LibraryVariableSetResource>,
        IFindByName<LibraryVariableSetResource>
    {
        Task<LibraryVariableSetEditor> CreateOrModify(string name);
        Task<LibraryVariableSetEditor> CreateOrModify(string name, string description);
    }

    class LibraryVariableSetRepository : BasicRepository<LibraryVariableSetResource>, ILibraryVariableSetRepository
    {
        public LibraryVariableSetRepository(IOctopusAsyncClient client)
            : base(client, "LibraryVariables")
        {
        }

        public Task<LibraryVariableSetEditor> CreateOrModify(string name)
        {
            return new LibraryVariableSetEditor(this, new VariableSetRepository(Client)).CreateOrModify(name);
        }

        public Task<LibraryVariableSetEditor> CreateOrModify(string name, string description)
        {
            return new LibraryVariableSetEditor(this, new VariableSetRepository(Client)).CreateOrModify(name, description);
        }
    }
}

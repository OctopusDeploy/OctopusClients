using System;
using System.Threading;
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
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<LibraryVariableSetEditor> CreateOrModify(string name);
        Task<LibraryVariableSetEditor> CreateOrModify(string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<LibraryVariableSetEditor> CreateOrModify(string name, string description);
        Task<LibraryVariableSetEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken);
    }

    class LibraryVariableSetRepository : BasicRepository<LibraryVariableSetResource>, ILibraryVariableSetRepository
    {
        public LibraryVariableSetRepository(IOctopusAsyncRepository repository)
            : base(repository, "LibraryVariables")
        {
        }

        public Task<LibraryVariableSetEditor> CreateOrModify(string name)
        {
            return CreateOrModify(name, CancellationToken.None);
        }

        public Task<LibraryVariableSetEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            return new LibraryVariableSetEditor(this, new VariableSetRepository(Repository)).CreateOrModify(name);
        }

        public Task<LibraryVariableSetEditor> CreateOrModify(string name, string description)
        {
            return CreateOrModify(name, description, CancellationToken.None);
        }

        public Task<LibraryVariableSetEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            return new LibraryVariableSetEditor(this, new VariableSetRepository(Repository)).CreateOrModify(name, description);
        }
    }
}

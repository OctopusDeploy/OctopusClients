using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ILibraryVariableSetRepository :
        ICreate<LibraryVariableSetResource>,
        IGet<LibraryVariableSetResource>,
        IModify<LibraryVariableSetResource>,
        IDelete<LibraryVariableSetResource>,
        IFindByName<LibraryVariableSetResource>
    {
        LibraryVariableSetEditor CreateOrModify(string name);
        LibraryVariableSetEditor CreateOrModify(string name, string description);
    }
    
    class LibraryVariableSetRepository : BasicRepository<LibraryVariableSetResource>, ILibraryVariableSetRepository
    {
        public LibraryVariableSetRepository(IOctopusClient client)
            : base(client, "LibraryVariables")
        {
        }

        public LibraryVariableSetEditor CreateOrModify(string name)
        {
            return new LibraryVariableSetEditor(this, new VariableSetRepository(Client)).CreateOrModify(name);
        }

        public LibraryVariableSetEditor CreateOrModify(string name, string description)
        {
            return new LibraryVariableSetEditor(this, new VariableSetRepository(Client)).CreateOrModify(name, description);
        }
    }
}
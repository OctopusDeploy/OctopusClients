using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITagSetRepository : ICreate<TagSetResource>, IModify<TagSetResource>, IGet<TagSetResource>, IDelete<TagSetResource>, IFindByName<TagSetResource>, IGetAll<TagSetResource>
    {
        void Sort(string[] tagSetIdsInOrder);
        TagSetEditor CreateOrModify(string name);
        TagSetEditor CreateOrModify(string name, string description);
    }
}
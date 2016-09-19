using System;
using System.Threading.Tasks;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITagSetRepository : ICreate<TagSetResource>, IModify<TagSetResource>, IGet<TagSetResource>, IDelete<TagSetResource>, IFindByName<TagSetResource>, IGetAll<TagSetResource>
    {
        Task Sort(string[] tagSetIdsInOrder);
        Task<TagSetEditor> CreateOrModify(string name);
        Task<TagSetEditor> CreateOrModify(string name, string description);
    }
}
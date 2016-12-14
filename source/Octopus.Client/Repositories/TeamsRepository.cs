using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITeamsRepository :
        ICreate<TeamResource>,
        IModify<TeamResource>,
        IDelete<TeamResource>,
        IFindByName<TeamResource>,
        IGet<TeamResource>
    {
    }
    
    class TeamsRepository : BasicRepository<TeamResource>, ITeamsRepository
    {
        public TeamsRepository(IOctopusClient client)
            : base(client, "Teams")
        {
        }
    }
}
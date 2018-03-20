using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
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
        public TeamsRepository(IOctopusAsyncClient client)
            : base(client, "Teams")
        {
        }
    }

    public interface ITeamsV2Repository :
        ICreate<TeamV2Resource>,
        IModify<TeamV2Resource>,
        IDelete<TeamV2Resource>,
        IFindByName<TeamV2Resource>,
        IGet<TeamV2Resource>
    {
    }

    class TeamsV2Repository : BasicRepository<TeamV2Resource>, ITeamsV2Repository
    {
        public TeamsV2Repository(IOctopusAsyncClient client)
            : base(client, "TeamsV2")
        {
        }
    }
}

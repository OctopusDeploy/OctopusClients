using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IUserRepository :
        IPaginate<UserResource>,
        IGet<UserResource>,
        IModify<UserResource>,
        IDelete<UserResource>
    {
        Task<UserResource> Register(RegisterCommand registerCommand);
        Task SignIn(LoginCommand loginCommand);
        Task SignOut();
        Task<UserResource> GetCurrent();
        Task<UserPermissionSetResource> GetPermissions(UserResource user);
        Task<ApiKeyResource> CreateApiKey(UserResource user, string purpose = null);
        Task<List<ApiKeyResource>> GetApiKeys(UserResource user);
        Task RevokeApiKey(ApiKeyResource apiKey);
        Task<InvitationResource> Invite(string addToTeamId);
        Task<InvitationResource> Invite(ReferenceCollection addToTeamIds);
    }
}
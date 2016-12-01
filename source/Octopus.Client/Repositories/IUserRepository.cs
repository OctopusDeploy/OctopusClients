using System;
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
        UserResource Register(RegisterCommand registerCommand);
        void SignIn(LoginCommand loginCommand);
        void SignIn(string username, string password, bool rememberMe = false);
        void SignOut();
        UserResource GetCurrent();
        UserPermissionSetResource GetPermissions(UserResource user);
        ApiKeyResource CreateApiKey(UserResource user, string purpose = null);
        List<ApiKeyResource> GetApiKeys(UserResource user);
        void RevokeApiKey(ApiKeyResource apiKey);
        InvitationResource Invite(string addToTeamId);
        InvitationResource Invite(ReferenceCollection addToTeamIds);
    }
}
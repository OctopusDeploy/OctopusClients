﻿using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class UsernamePasswordAccountEditor : AccountEditor<UsernamePasswordAccountResource, UsernamePasswordAccountEditor>
    {
        public UsernamePasswordAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}
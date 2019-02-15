using System;
using Octopus.Client.Model;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// An exception thrown when using a Permission that is not supported by the connection version of Octopus Server
    /// </summary>
    public class PermissionNotSupportedException : Exception
    {
        public PermissionNotSupportedException(Permission permission) : base($"The {permission:G} permission is not supported in this version of Octopus Server")
        {
        }
    }
}
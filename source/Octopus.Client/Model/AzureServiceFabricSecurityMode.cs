using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Model
{
    public enum AzureServiceFabricSecurityMode
    {
        Unsecure,
        SecureClientCertificate,
        SecureAzureAD,
    }
}

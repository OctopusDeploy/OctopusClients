using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Model.Endpoints
{
    /// <summary>
    /// The base resource for any object representing one of many kinds of
    /// authentication and associated metadata.
    /// </summary>
    public interface IEndpointWithMultipleAuthenticationResource
    {
        /// <summary>
        /// The field used to map the interface to a concrete class via nevermore
        /// </summary>
        string AuthenticationType { get; }
    }
}

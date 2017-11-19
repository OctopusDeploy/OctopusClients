using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories
{
    public interface IConfigurationRepository
    {
        T Get<T>() where T : class, IResource, new();
        void Modify<T>(T configurationResource) where T : class, IResource, new();
    }
}

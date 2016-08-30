using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IFeaturesConfigurationRepository
    {
        FeaturesConfigurationResource GetFeaturesConfiguration();
        FeaturesConfigurationResource ModifyFeaturesConfiguration(FeaturesConfigurationResource resource);
    }
}
﻿namespace Octopus.Client.Extensibility
{
    public interface IResource
    {
        string Id { get; }

        LinkCollection Links { get; set; }
    }

    public interface INamedResource
    {
        string Name { get; }
    }

    public interface IHaveSpaceResource
    {
        string SpaceId { get; set; }
    }
}
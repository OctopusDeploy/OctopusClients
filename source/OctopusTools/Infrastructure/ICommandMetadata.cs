using System;

namespace OctopusTools.Infrastructure
{
    public interface ICommandMetadata
    {
        string Name { get; }
        string[] Aliases { get; }
        string Description { get; }
    }
}
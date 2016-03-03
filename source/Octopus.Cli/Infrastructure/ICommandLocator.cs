using System;

namespace OctopusTools.Infrastructure
{
    public interface ICommandLocator
    {
        ICommandMetadata[] List();
        ICommand Find(string name);
    }
}
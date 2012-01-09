using System;

namespace OctopusTools.Infrastructure
{
    public interface ICommandProcessor
    {
        void Process(string[] args);
    }
}
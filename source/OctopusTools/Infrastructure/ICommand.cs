using System;
using OctopusTools.Commands;

namespace OctopusTools.Infrastructure
{
    public interface ICommand
    {
        OptionSet Options { get; }

        void Execute();
    }
}

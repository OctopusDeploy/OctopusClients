using System;
using System.Collections.Generic;

namespace OctopusTools.Infrastructure
{
    public interface ICommandLineArgsProvider
    {
        IEnumerable<string> Args { get; set; }
    }
}

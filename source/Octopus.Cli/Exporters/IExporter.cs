using System;

namespace Octopus.Cli.Exporters
{
    public interface IExporter
    {
        void Export(params string[] parameters);
    }
}
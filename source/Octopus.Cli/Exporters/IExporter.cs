using System;

namespace OctopusTools.Exporters
{
    public interface IExporter
    {
        void Export(params string[] parameters);
    }
}
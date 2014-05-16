using System;

namespace OctopusTools.Importers
{
    public interface IImporter
    {
        void Import(params string[] parameters);
    }
}
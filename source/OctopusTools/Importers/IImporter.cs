using System;

namespace OctopusTools.Importers
{
    public interface IImporter
    {
        bool Validate(params string[] parameters);
        void Import(params string[] parameters);
    }
}
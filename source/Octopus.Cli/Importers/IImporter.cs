using System;

namespace Octopus.Cli.Importers
{
    public interface IImporter
    {
        bool Validate(params string[] parameters);
        void Import(params string[] parameters);
    }
}
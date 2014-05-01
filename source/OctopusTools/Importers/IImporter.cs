using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Importers
{
    public interface IImporter
    {
        void Import(string filePath);
    }
}

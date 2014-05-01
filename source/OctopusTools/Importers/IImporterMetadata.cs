using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Importers
{
    public interface IImporterMetadata
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}

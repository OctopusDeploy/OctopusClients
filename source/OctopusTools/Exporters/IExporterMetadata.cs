using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OctopusTools.Exporters
{
    public interface IExporterMetadata
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}

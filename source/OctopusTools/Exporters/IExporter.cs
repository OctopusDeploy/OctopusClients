using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.UI.WebControls.WebParts;

namespace OctopusTools.Exporters
{
    public interface IExporter
    {
        void Export(params string[] parameters);
    }
}

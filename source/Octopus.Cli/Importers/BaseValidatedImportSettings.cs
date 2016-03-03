using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Importers
{
    internal abstract class BaseValidatedImportSettings
    {
        protected BaseValidatedImportSettings()
        {
            ErrorList = new List<string>();
        }

        public IEnumerable<string> ErrorList { get; set; }
        public bool HasErrors { get { return ErrorList.Any(); } }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuke.Common.IO;

namespace tools
{
    public static class FileSystem
    {
        public static string[] GetFiles(AbsolutePath path, string filter)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path!);
                return Array.Empty<string>();
            }

            return Directory.GetFiles(path, filter);
        }
    }
}

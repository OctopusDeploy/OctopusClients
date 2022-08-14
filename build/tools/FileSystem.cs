using System;
using System.IO;
using Nuke.Common.IO;

namespace Tools
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

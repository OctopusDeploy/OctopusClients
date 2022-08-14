using System;
using System.IO;
using Nuke.Common.IO;

namespace Tools
{
    public static class FileSystem
    {
        public static string[] GetFiles(AbsolutePath path, string filter)
        {
            return Directory.GetFiles(path, filter);
        }
    }
}

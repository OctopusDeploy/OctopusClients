using System;
using System.IO;

namespace Octopus.Client.Model
{
    public class FileUpload
    {
        public Stream Contents { get; set; }

        public string FileName { get; set; }
    }
}
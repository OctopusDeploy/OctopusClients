using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json;
using NuGet;
using Octopus.Platform.Util;
using OctopusTools.Infrastructure;

namespace OctopusTools.Importers
{
    public class FileSystemImporter
    {
        readonly IOctopusFileSystem fileSystem;
        readonly ILog log;

        public FileSystemImporter(IOctopusFileSystem fileSystem, ILog log)
        {
            this.fileSystem = fileSystem;
            this.log = log;
        }

        public T Import<T>(string filePath)
        {
            if (!fileSystem.FileExists(filePath))
                throw new CommandException("Unable to find the specified export file");

            var export = fileSystem.ReadFile(filePath);

            log.Debug("Export file successfully loaded");

            return JsonConvert.DeserializeObject<T>(export);
        }
    }
}

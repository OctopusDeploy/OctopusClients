using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using log4net;
using Newtonsoft.Json;
using NuGet;
using Octopus.Client.Serialization;
using Octopus.Platform.Util;

namespace OctopusTools.Exporters
{
    public class FileSystemExporter
    {
        readonly IOctopusFileSystem fileSystem;
        readonly ILog log;

        public FileSystemExporter(IOctopusFileSystem fileSystem, ILog log)
        {
            this.fileSystem = fileSystem;
            this.log = log;
        }

        public void Export(string filePath, object metadata, object exportObject)
        {
            var serializedObject = JsonConvert.SerializeObject(new {Meta = metadata, Object = exportObject}, JsonSerialization.GetDefaultSerializerSettings());
            fileSystem.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(serializedObject));

            log.DebugFormat("Export file {0} successfully created.", filePath);

        }
    }
}

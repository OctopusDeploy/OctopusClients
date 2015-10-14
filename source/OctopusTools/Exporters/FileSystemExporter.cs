using System;
using System.Text;
using log4net;
using Newtonsoft.Json;
using Octopus.Client.Serialization;
using Octopus.Shared.Util;
using OctopusTools.Extensions;

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

        public void Export<T>(string filePath, ExportMetadata metadata, T exportObject)
        {
            var x = exportObject.ToDynamic(metadata);

            var serializerSettings = JsonSerialization.GetDefaultSerializerSettings();
            var serializedObject = JsonConvert.SerializeObject(x, serializerSettings);

            fileSystem.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(serializedObject));

            log.DebugFormat("Export file {0} successfully created.", filePath);
        }
    }
}
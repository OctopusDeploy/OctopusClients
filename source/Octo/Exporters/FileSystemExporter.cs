using System.Text;
using Newtonsoft.Json;
using Octopus.Cli.Extensions;
using Octopus.Cli.Util;
using Octopus.Client.Serialization;
using Serilog;

namespace Octopus.Cli.Exporters
{
    public class FileSystemExporter
    {
        readonly IOctopusFileSystem fileSystem;
        readonly ILogger log;

        public FileSystemExporter(IOctopusFileSystem fileSystem, ILogger log)
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

            log.Debug("Export file {Path} successfully created.", filePath);
        }
    }
}
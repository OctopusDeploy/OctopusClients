using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Serialization;
using Serilog;

namespace Octopus.Cli.Importers
{
    public class FileSystemImporter
    {
        readonly IOctopusFileSystem fileSystem;
        readonly ILogger log;

        public FileSystemImporter(IOctopusFileSystem fileSystem, ILogger log)
        {
            this.fileSystem = fileSystem;
            this.log = log;
        }

        public T Import<T>(string filePath, string entityType)
        {
            if (!fileSystem.FileExists(filePath))
                throw new CommandException("Unable to find the specified export file");

            var export = fileSystem.ReadFile(filePath);

            log.Debug("Export file successfully loaded");

            var expando = JsonConvert.DeserializeObject<ExpandoObject>(export, JsonSerialization.GetDefaultSerializerSettings());
            var importedObject = expando as IDictionary<string, object>;
            if (importedObject == null ||
                !importedObject.ContainsKey("$Meta") ||
                (importedObject["$Meta"] as dynamic).ContainerType != entityType)
            {
                throw new CommandException("The data is not a valid " + entityType);
            }
            importedObject.Remove("$Meta");

            object exportedObject = null;
            if (importedObject.ContainsKey("Items"))
            {
                exportedObject = importedObject["Items"];
            }

            var serializedObject = JsonConvert.SerializeObject(exportedObject ?? expando);
            return JsonConvert.DeserializeObject<T>(serializedObject, JsonSerialization.GetDefaultSerializerSettings());
        }
    }
}
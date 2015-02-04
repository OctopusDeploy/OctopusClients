using System;
using System.Collections.Generic;
using System.Dynamic;
using log4net;
using Newtonsoft.Json;
using Octopus.Client.Serialization;
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
            
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(exportedObject ?? expando));
        }
    }
}
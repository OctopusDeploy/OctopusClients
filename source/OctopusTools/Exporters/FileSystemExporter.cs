using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Autofac.Features.Metadata;
using log4net;
using log4net.Util;
using Newtonsoft.Json;
using NuGet;
using Octopus.Client.Serialization;
using Octopus.Platform.Util;
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

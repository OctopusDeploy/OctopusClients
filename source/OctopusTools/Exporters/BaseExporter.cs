using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Platform.Util;
using OctopusTools.Commands;

namespace OctopusTools.Exporters
{
    public abstract class BaseExporter : IExporter
    {
        readonly FileSystemExporter fileSystemExporter;
        readonly ILog log;
        readonly IOctopusRepository repository;

        public ILog Log { get { return log; } }
        public IOctopusRepository Repository { get { return repository; } }
        public FileSystemExporter FileSystemExporter { get { return fileSystemExporter; } }
        public string FilePath { get; protected set; }

        protected BaseExporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
        {
            this.log = log;
            this.repository = repository;
            fileSystemExporter = new FileSystemExporter(fileSystem, log);
        }

        public void Export(params string[] parameters)
        {
            var parameterDictionary = ParseParameters(parameters);
            FilePath = parameterDictionary["FilePath"];

            Export(parameterDictionary);
        }

        protected virtual void Export(Dictionary<string, string> paramDictionary)
        {
            
        }

        private Dictionary<string, string> ParseParameters(IEnumerable<string> parameters)
        {
            var paramDictionary = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                var values = parameter.Split(new[] { '=' });
                paramDictionary.Add(values[0], values[1]);
            }
            return paramDictionary;
        }

    }
}

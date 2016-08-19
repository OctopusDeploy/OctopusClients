using System;
using System.Collections.Generic;
using Serilog;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Exporters
{
    public abstract class BaseExporter : IExporter
    {
        protected BaseExporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILogger log)
        {
            this.Log = log;
            this.Repository = repository;
            FileSystemExporter = new FileSystemExporter(fileSystem, log);
        }

        public ILogger Log { get; }

        public IOctopusRepository Repository { get; }

        public FileSystemExporter FileSystemExporter { get; }

        public string FilePath { get; protected set; }

        public void Export(params string[] parameters)
        {
            var parameterDictionary = ParseParameters(parameters);
            FilePath = parameterDictionary["FilePath"];

            Export(parameterDictionary);
        }

        protected virtual void Export(Dictionary<string, string> paramDictionary)
        {
        }

        Dictionary<string, string> ParseParameters(IEnumerable<string> parameters)
        {
            var paramDictionary = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                var values = parameter.Split(new[] {'='});
                paramDictionary.Add(values[0], values[1]);
            }
            return paramDictionary;
        }
    }
}
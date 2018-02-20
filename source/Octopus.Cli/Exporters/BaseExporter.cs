using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Cli.Util;
using Octopus.Client;
using Serilog;

namespace Octopus.Cli.Exporters
{
    public abstract class BaseExporter : IExporter
    {
        protected BaseExporter(IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ILogger log)
        {
            this.Log = log;
            this.Repository = repository;
            FileSystemExporter = new FileSystemExporter(fileSystem, log);
        }

        public ILogger Log { get; }

        public IOctopusAsyncRepository Repository { get; }

        public FileSystemExporter FileSystemExporter { get; }

        public string FilePath { get; protected set; }

        public Task Export(params string[] parameters)
        {
            var parameterDictionary = ParseParameters(parameters);
            FilePath = parameterDictionary["FilePath"];

            return Export(parameterDictionary);
        }

        protected virtual Task Export(Dictionary<string, string> paramDictionary)
        {
            return Task.WhenAll();
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
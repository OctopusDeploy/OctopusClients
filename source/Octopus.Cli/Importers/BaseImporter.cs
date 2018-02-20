using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Cli.Util;
using Octopus.Client;
using Serilog;

namespace Octopus.Cli.Importers
{
    public abstract class BaseImporter : IImporter
    {
        protected BaseImporter(IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ILogger log)
        {
            this.Log = log;
            this.Repository = repository;
            FileSystemImporter = new FileSystemImporter(fileSystem, log);
        }

        public ILogger Log { get; }

        public IOctopusAsyncRepository Repository { get; }

        public FileSystemImporter FileSystemImporter { get; }

        public string FilePath { get; set; }

        public Task<bool> Validate(params string[] parameters)
        {
            var paramDictionary = ParseParameters(parameters);
            FilePath = paramDictionary["FilePath"];

            return Validate(paramDictionary);
        }

        public Task Import(params string[] parameters)
        {
            var paramDictionary = ParseParameters(parameters);
            FilePath = paramDictionary["FilePath"];

            return Import(paramDictionary);
        }

        protected virtual Task Import(Dictionary<string, string> paramDictionary)
        {
            return Task.WhenAll();
        }

        protected virtual Task<bool> Validate(Dictionary<string, string> paramDictionary)
        {
            return Task.FromResult(true);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Web.UI;
using log4net;
using Octopus.Client;
using Octopus.Platform.Util;
using OctopusTools.Infrastructure;

namespace OctopusTools.Importers
{
    public abstract class BaseImporter : IImporter
    {
        readonly FileSystemImporter fileSystemImporter;
        readonly ILog log;
        readonly IOctopusRepository repository;

        public ILog Log { get { return log; } }
        public IOctopusRepository Repository { get { return repository; } }
        public FileSystemImporter FileSystemImporter { get { return fileSystemImporter; } }
        public string FilePath { get; set; }

        protected BaseImporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
        {
            this.log = log;
            this.repository = repository;
            fileSystemImporter = new FileSystemImporter(fileSystem, log);
        }

        public void Import(params string[] parameters)
        {
            var paramDictionary = ParseParameters(parameters);
            FilePath = paramDictionary["FilePath"];

            Import(paramDictionary);
        }

        protected virtual void Import(Dictionary<string, string> paramDictionary)
        { }

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

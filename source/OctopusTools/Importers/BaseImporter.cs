using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using log4net;
using Octopus.Client;
using Octopus.Platform.Util;

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
        public string FilePath { get; protected set; }

        protected BaseImporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
        {
            this.log = log;
            this.repository = repository;
            fileSystemImporter = new FileSystemImporter(fileSystem, log);
        }

        public virtual void Import(string filePath)
        {
        }
    }
}

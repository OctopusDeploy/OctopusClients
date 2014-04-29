using log4net;
using Octopus.Platform.Util;
using OctopusTools.Infrastructure;
using System;
using System.IO;
using System.Text;

namespace OctopusTools.Commands
{
    public abstract class BaseImportCommand : ApiCommand
    {
        readonly IOctopusFileSystem fileSystem;

        protected BaseImportCommand(IOctopusFileSystem fileSystem, IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
            this.fileSystem = fileSystem;
        }

        public string GetSerializedObjectFromFile(string filePath)
        {
            Log.Debug("Loading export file");
            if (!fileSystem.FileExists(filePath))
                throw new CommandException("Unable to find the specified export file");

            string export;
            try
            {
                var file = fileSystem.OpenFile(filePath, FileAccess.Read);
                var bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                export = Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                throw new CommandException("Unable to read the specified export file. Error: " + ex.Message);
            }

            Log.Debug("Export file loaded");
            return export;
        }
    }
}

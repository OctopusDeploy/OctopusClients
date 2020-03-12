using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using Octopus.Client.Logging;

namespace Octopus.Client
{
#if FULL_FRAMEWORK
    class LocationChecker
    {
        private static readonly ILog Logger = LogProvider.For<LocationChecker>();

        public static void CheckAssemblyLocation()
        {
            var octopusRegNode = Registry.LocalMachine.OpenSubKey(@"Software\Octopus");
            var serverRegNode = octopusRegNode.OpenSubKey("Server");
            if (serverRegNode != null)
            {
                var serverInstallationFolder = (string)serverRegNode.GetValue("InstallLocation");
                var currentAssemblyLocation = typeof(LocationChecker).Assembly.Location;

                if (currentAssemblyLocation.Contains(serverInstallationFolder))
                {
                    var warningMessage = $"Using Octopus.Client from the server's installation folder has been deprecated. In future versions it may not be shipped with server.";
                    Logger.Warn(warningMessage);
                    Console.WriteLine(warningMessage);
                }
            }
        }
    }
#endif
}
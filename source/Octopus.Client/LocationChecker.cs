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
            using (var octopusRegNode = Registry.LocalMachine.OpenSubKey(@"Software\Octopus"))
            {
                CheckBasedOnApplication(octopusRegNode, "Server");
                CheckBasedOnApplication(octopusRegNode, "Tentacle");
            }
        }

        private static void CheckBasedOnApplication(RegistryKey octopusRegNode, string application)
        {
            using (var applicationRegNode = octopusRegNode.OpenSubKey(application))
            {
                if (applicationRegNode != null)
                {
                    var installationFolder = (string) applicationRegNode.GetValue("InstallLocation");
                    var currentAssemblyLocation = typeof(LocationChecker).Assembly.Location;

                    if (currentAssemblyLocation.Contains(installationFolder))
                    {
                        var warningMessage =
                            $"Using Octopus.Client from the {application}'s installation folder has been deprecated. In future versions it may not be shipped with {application}. Please see https://g.octopushq.com/usingoctopusclientfrominstallationfolder.";

                        // Write this to the log, if one is configured. Also write it to the console in case no log has been configured
                        Logger.Warn(warningMessage);
                        Console.WriteLine(warningMessage);
                    }
                }
            }
        }
    }
#endif
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model
{
    internal class EnvironmentHelper : IEnvironmentHelper
    {
        public string[] SafelyGetEnvironmentInformation()
        {
            var envVars = GetEnvironmentVars();
            return envVars.ToArray();
        }

        static string SafelyGet(Func<string> thingToGet)
        {
            try
            {
                return thingToGet.Invoke();
            }
            catch (Exception)
            {
                return "Unable to retrieve environment information.";
            }
        }

        static IEnumerable<string> GetEnvironmentVars()
        {
            yield return SafelyGet(() => $"{Environment.OSVersion}");
            yield return SafelyGet(() => $"{(Environment.Is64BitOperatingSystem ? "x64" : "x86")}");
            if (ExecutionEnvironment.IsRunningOnNix || ExecutionEnvironment.IsRunningOnMac)
                yield return SafelyGet(() => $"mono");
        }
    }

    internal interface IEnvironmentHelper
    {
        string[] SafelyGetEnvironmentInformation();
    }
}
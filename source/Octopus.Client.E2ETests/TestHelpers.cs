using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Octopus.Client.E2ETests
{
    public static class TestHelpers
    {
        internal static string GetNuGetPackage()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullLocalPath());
            var artifactsFolder = Path.GetFullPath(Path.Combine(path, @"..\..\..\..\..\artifacts"));
            var package = Directory.EnumerateFiles(artifactsFolder, "Octopus.Client.*.nupkg").FirstOrDefault();
            if (package == null)
                Assert.Fail($"Couldn't find built nuget package with name 'Octopus.Client.*.nupkg' at '{artifactsFolder}'");
            return package;
        }

        internal static string GetRuntime()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullLocalPath());
            var runtime = new DirectoryInfo(path).Name.Replace("netcoreapp", "netstandard");
            return runtime;
        }
    }
}

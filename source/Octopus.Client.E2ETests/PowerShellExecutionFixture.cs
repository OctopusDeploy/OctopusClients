using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.E2ETests
{
    /// <summary>
    /// These tests are designed to ensure that we keep our published artifact working in the way we expect it to
    ///  * is a single dll that can be referenced from powershell
    ///  * inlines dependencies properly
    ///  * doesn't pull down unnecessary dependencies
    /// If you find yourself changing these tests, it's very likely you'll need to change
    /// https://github.com/OctopusDeploy/docs/blob/master/docs/octopus-rest-api/octopus.client.md 
    /// </summary>
    [TestFixture]
    public class PowerShellExecutionFixture
    {
#if SUPPORTS_ILMERGE            
        [Test]
        public void CanBeRunFromPowerShell()
        {
            var tempFolder = GetTempFolder();
            var extractedDll = ExtractDll(tempFolder);
            var powerShellFile = CreatePowerShellFile(extractedDll, tempFolder);

            var output = SilentProcessRunner.ExecuteCommand("powershell", $" -NoProfile -NoLogo -NonInteractive -WindowStyle Hidden -ExecutionPolicy Unrestricted -File {powerShellFile}");
            output.TrimEnd('\r', '\n').Should().Be("Octopus Deploy");
        }
#endif

        private static string CreatePowerShellFile(string extractedDll, string tempFolder)
        {
            //TODO: Review which public server this is referencing. If that server is down, these tests will fail :(
            var powerShellFileContent = @"
Add-Type -Path '" + extractedDll + @"'
$server = ""https://deploy.octopushq.com/""
$apiKey = $null
$endpoint = New-Object Octopus.Client.OctopusServerEndpoint($server, $apiKey)
$repository = New-Object Octopus.Client.OctopusRepository($endpoint)
$repository.LoadRootDocument().Application";
            
            var powerShellFile = Path.Combine(tempFolder, "Octopus.Client.Test.ps1");
            File.WriteAllText(powerShellFile, powerShellFileContent);
            return powerShellFile;
        }

        private static string ExtractDll(string tempFolder)
        {
            var package = TestHelpers.GetNuGetPackage();
            string extractedDll;
            var runtime = TestHelpers.GetRuntime();
            using (var zipFile = ZipFile.OpenRead(package))
            {
                var zipArchiveEntry = zipFile.Entries.FirstOrDefault(x => x.FullName.Contains($"lib/{runtime}/Octopus.Client.dll"));
                if (zipArchiveEntry == null)
                    Assert.Fail($"Unable to find 'lib/{runtime}/Octopus.Client.dll' in the nupkg file.");
                using (var stream = zipArchiveEntry.Open())
                {
                    extractedDll = Path.Combine(tempFolder, "Octopus.Client.dll");
                    using (var file = File.OpenWrite(extractedDll))
                        stream.CopyTo(file);
                }
            }

            return extractedDll;
        }

        private static string GetTempFolder()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            return tempFolder;
        }
    }
}

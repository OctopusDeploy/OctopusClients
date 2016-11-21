using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Cli.Tests.Integration
{
    [TestFixture]
    public class PackTests
    {
        [Test]
        [TestCase("nupkg", "1.0.0.0", Description = "NuGet should retain four-part versions")]
        [TestCase("nupkg", "2016.02.01.09", Description = "NuGet should retain four-part versions with leading zeros")]
        [TestCase("zip", "1.0.0.0", Description = "Zip should retain four-part versions")]
        [TestCase("zip", "2016.02.01.09", Description = "Zip should retain four-part versions with leading zeros")]
        public void Execute(string format, string version)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            try
            {
                var rootDirectory = Directory.CreateDirectory(tempFolder);
                var inputDirectory = Directory.CreateDirectory(Path.Combine(rootDirectory.FullName, "input"));
                var packagedDirectory = Directory.CreateDirectory(Path.Combine(rootDirectory.FullName, "packaged"));
                var extractedDirectory = Directory.CreateDirectory(Path.Combine(rootDirectory.FullName, "extracted"));
                File.WriteAllText(Path.Combine(inputDirectory.FullName, "Test.txt"), "Test");
                File.WriteAllText(Path.Combine(inputDirectory.FullName, "Test[squarebrackets].txt"), "Test");

                var result = Program.Run(
                    new[]
                    {
                        "pack",
                        "--id=TestPackage",
                        $"--basePath={inputDirectory.FullName}",
                        $"--outFolder={packagedDirectory.FullName}",
                        $"--version={version}",
                        $"--format={format}"
                    }
                );

                result.Should().Be(0);
                var expectedOutputFilePath = Path.Combine(packagedDirectory.FullName, $"TestPackage.{version}.{format}");
                File.Exists(expectedOutputFilePath)
                    .Should()
                    .BeTrue("the package should have been given the right name.");

                // TODO: It would be really nice to have a simple end-to-end test that proves we can pack/unpack files preserving filenames and timestamps etc...
                //var calamariResult = SilentProcessRunner.ExecuteCommand(@"C:\dev\Calamari\source\Calamari\bin\Debug\netcoreapp1.0\win7-x64\Calamari.exe",
                //    $"extract-package --package={expectedOutputFilePath} --target={extractedDirectory.FullName}", rootDirectory.FullName, Console.WriteLine, Console.WriteLine);
                //calamariResult.Should().Be(0);

                //var extractedFiles = Directory.GetFiles(extractedDirectory.FullName, "*", SearchOption.AllDirectories).Select(x => x.Replace(extractedDirectory.FullName, ""));
                //var inputFiles = Directory.GetFiles(inputDirectory.FullName, "*", SearchOption.AllDirectories).Select(x => x.Replace(inputDirectory.FullName, ""));
                //extractedFiles.ShouldAllBeEquivalentTo(inputFiles);
            }
            finally
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }

    public static class SilentProcessRunner
    {
        // ReSharper disable once InconsistentNaming
        private const int CP_OEMCP = 1;
        private static readonly Encoding oemEncoding;

        static SilentProcessRunner()
        {
            try
            {
                CPINFOEX info;
                if (GetCPInfoEx(CP_OEMCP, 0, out info))
                {
                    oemEncoding = Encoding.GetEncoding(info.CodePage);
                }
                else
                {
                    oemEncoding = Encoding.GetEncoding(850);
                }
            }
            catch (Exception)
            {
                Trace.WriteLine("Couldn't get default OEM encoding");
                oemEncoding = Encoding.UTF8;
            }
        }

        public static int ExecuteCommand(string executable, string arguments, string workingDirectory,
            Action<string> output, Action<string> error)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = executable;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.WorkingDirectory = workingDirectory;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.StandardOutputEncoding = oemEncoding;
                    process.StartInfo.StandardErrorEncoding = oemEncoding;

                    using (var outputWaitHandle = new AutoResetEvent(false))
                    using (var errorWaitHandle = new AutoResetEvent(false))
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                output(e.Data);
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                error(e.Data);
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();

                        outputWaitHandle.WaitOne();
                        errorWaitHandle.WaitOne();

                        return process.ExitCode;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error when attempting to execute {0}: {1}", executable, ex.Message),
                    ex);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCPInfoEx([MarshalAs(UnmanagedType.U4)] int CodePage,
            [MarshalAs(UnmanagedType.U4)] int dwFlags, out CPINFOEX lpCPInfoEx);

        private const int MAX_DEFAULTCHAR = 2;
        private const int MAX_LEADBYTES = 12;
        private const int MAX_PATH = 260;

        [StructLayout(LayoutKind.Sequential)]
        private struct CPINFOEX
        {
            [MarshalAs(UnmanagedType.U4)] public int MaxCharSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_DEFAULTCHAR)] public byte[] DefaultChar;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_LEADBYTES)] public byte[] LeadBytes;

            public char UnicodeDefaultChar;

            [MarshalAs(UnmanagedType.U4)] public int CodePage;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)] public string CodePageName;
        }
    }
}
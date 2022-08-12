using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Serilog;
using Serilog.Core;

namespace tools
{
    public static class DockerCompose
    {
        static string DockerComposePath =>
            ToolPathResolver.GetPathExecutable("docker-compose");
        
        public static IReadOnlyCollection<Output> DockerComposeUp(AbsolutePath file, string arguments = "",  AbsolutePath workingDirectory = null)
            => ExecuteDockerCompose(file, arguments, workingDirectory, "up");


        public static IReadOnlyCollection<Output> DockerComposeDown(AbsolutePath file, string arguments = "", AbsolutePath workingDirectory = null)
            => ExecuteDockerCompose(file, arguments, workingDirectory, "down");

        public static IReadOnlyCollection<Output> DockerComposeBuild(AbsolutePath file, string arguments ="", AbsolutePath workingDirectory = null)
            => ExecuteDockerCompose(file, arguments, workingDirectory, "build");

        static IReadOnlyCollection<Output> ExecuteDockerCompose(AbsolutePath file, string userArguments, AbsolutePath workingDirectory, string command)
        {
            var arguments = string.Concat("-f ",file, " ", command, " ", userArguments);

            var process = ProcessTasks.StartProcess(DockerComposePath, arguments, workingDirectory, customLogger: DockerLogger);
            process.AssertZeroExitCode();
            return process.Output;
        }

        static void DockerLogger(OutputType type, string output)
        {
            if (type == OutputType.Err && !output.Contains("failed"))
            {
                Log.Information(output);
            }
            else
            {
                ProcessTasks.DefaultLogger(type, output);
            }
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Octopus.Client.Model.DeploymentProcess
{
    public class InlineScriptActionFromFileInAssembly : ScriptAction
    {
        public InlineScriptActionFromFileInAssembly(string resourceName, Assembly resourceAssembly = null)
        {
            ResourceAssembly = resourceAssembly ?? Assembly.GetEntryAssembly();
            var candidates = ResourceAssembly.GetManifestResourceNames().Where(name => name.EndsWith(resourceName)).ToArray();
            if (!candidates.Any())
                throw new ArgumentException($"There is no embedded resource in {ResourceAssembly.GetName().Name} like {resourceName}. Available resources are:{Environment.NewLine}{string.Join(Environment.NewLine, ResourceAssembly.GetManifestResourceNames().OrderBy(x => x))}");

            if (candidates.Length > 1)
                throw new ArgumentException($"There are {candidates.Length} embedded resources in {ResourceAssembly.GetName().Name} like {resourceName}, which one do you want?{Environment.NewLine}{string.Join(Environment.NewLine, candidates.OrderBy(x => x))}");

            ResourceName = candidates.Single();
        }

        public override ScriptSource Source => ScriptSource.Inline;
        public override ScriptSyntax Syntax => CalculateScriptType(ResourceName);
        public Assembly ResourceAssembly { get; }
        public string ResourceName { get; }
        public string GetScriptBody()
        {
            using (var stream = ResourceAssembly.GetManifestResourceStream(ResourceName))
            {
                if (stream == null) throw new ArgumentException(nameof(ResourceName), $"There is no Embedded Resource '{ResourceName}' in {ResourceAssembly}");
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
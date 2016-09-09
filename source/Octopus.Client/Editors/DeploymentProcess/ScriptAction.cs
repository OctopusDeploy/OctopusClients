using System;
using System.IO;
using System.Reflection;
using Octopus.Client.Model;

namespace Octopus.Client.Editors.DeploymentProcess
{
    public abstract class ScriptAction
    {
        public abstract ScriptSource Source { get; }
        public abstract ScriptSyntax Syntax { get; }

        protected ScriptSyntax CalculateScriptType(string scriptFileName)
        {
            if (string.Equals(Path.GetExtension(scriptFileName), ".ps1", StringComparison.OrdinalIgnoreCase)) return ScriptSyntax.PowerShell;
            if (string.Equals(Path.GetExtension(scriptFileName), ".csx", StringComparison.OrdinalIgnoreCase)) return ScriptSyntax.CSharp;
            if (string.Equals(Path.GetExtension(scriptFileName), ".sh", StringComparison.OrdinalIgnoreCase)) return ScriptSyntax.Bash;
            if (string.Equals(Path.GetExtension(scriptFileName), ".fsx", StringComparison.OrdinalIgnoreCase)) return ScriptSyntax.FSharp;
            throw new NotSupportedException($"{scriptFileName} is not one of the well known script types supported by Octopus Deploy.");
        }

        public static InlineScriptAction InlineScript(ScriptSyntax syntax, string scriptBody)
        {
            return new InlineScriptAction(syntax, scriptBody);
        }

        public static InlineScriptActionFromFileInAssembly InlineScriptFromFileInAssembly(string resourceName, Assembly resourceAssembly = null)
        {
            return new InlineScriptActionFromFileInAssembly(resourceName, resourceAssembly);
        }
    }
}
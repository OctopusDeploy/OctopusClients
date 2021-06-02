using System;

namespace Octopus.Client.Model
{
    public static class ScriptSyntaxExtension
    {
        public static string GetExtension(this ScriptSyntax syntax)
        {
            switch (syntax)
            {
                case ScriptSyntax.PowerShell:
                    return "ps1";
                case ScriptSyntax.Bash:
                    return "sh";
                case ScriptSyntax.CSharp:
                    return "csx";
                case ScriptSyntax.FSharp:
                    return "fsx";
                case ScriptSyntax.Python:
                    return "py";
                default:
                    throw new ArgumentOutOfRangeException("syntax");
            }
        }
    }
}
using System;
using System.Linq;

namespace Octopus.Client.Model
{
    public class ScriptSyntaxMeta
    {
        public readonly ScriptSyntax ScriptSyntax;
        public readonly string Extension;
        public readonly string Name;

        ScriptSyntaxMeta(ScriptSyntax scriptSyntax, string extension, string name)
        {
            ScriptSyntax = scriptSyntax;
            Extension = extension;
            Name = name;
        }

        public static ScriptSyntaxMeta FromName(string name)
        {
            return All.Where(x => x.Name == name).DefaultIfEmpty(PowerShell).First();
        }

        public static ScriptSyntaxMeta FromExtension(string extension)
        {
            extension = extension.TrimStart('.');
            return All.FirstOrDefault(x => x.Extension == extension);
        }

        public static readonly ScriptSyntaxMeta PowerShell = new ScriptSyntaxMeta(ScriptSyntax.PowerShell, "ps1", "PowerShell");
        public static readonly ScriptSyntaxMeta Bash = new ScriptSyntaxMeta(ScriptSyntax.Bash, "sh", "Bash");
        public static readonly ScriptSyntaxMeta CSharp = new ScriptSyntaxMeta(ScriptSyntax.CSharp, "csx", "CSharp");
        public static readonly ScriptSyntaxMeta FSharp = new ScriptSyntaxMeta(ScriptSyntax.FSharp, "fsx", "FSharp");
        public static readonly ScriptSyntaxMeta Python = new ScriptSyntaxMeta(ScriptSyntax.Python, "py", "Python");
        public static ScriptSyntaxMeta[] All = {PowerShell, Bash, CSharp, FSharp, Python};
    }
}
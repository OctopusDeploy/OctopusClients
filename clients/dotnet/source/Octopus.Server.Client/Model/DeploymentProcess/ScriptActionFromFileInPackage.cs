using System;

namespace Octopus.Client.Model.DeploymentProcess
{
    public class ScriptActionFromFileInPackage : ScriptAction
    {
        public ScriptActionFromFileInPackage(PackageResource package, string scriptFilePath)
        {
            PackageFeedId = package.FeedId;
            PackageId = package.PackageId;
            ScriptFilePath = scriptFilePath;
        }

        public override ScriptSource Source => ScriptSource.Package;
        public override ScriptSyntax Syntax => CalculateScriptType(ScriptFilePath);
        public string PackageFeedId { get; protected set; }
        public string PackageId { get; protected set; }
        public string ScriptFilePath { get; protected set; }
    }
}
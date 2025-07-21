using System;

namespace Octopus.Client.Model.DeploymentProcess
{
    public class InlineScriptAction : ScriptAction
    {
        private readonly string scriptBody;

        public InlineScriptAction(ScriptSyntax syntax, string scriptBody)
        {
            this.scriptBody = scriptBody;
            Syntax = syntax;
        }

        public override ScriptSource Source => ScriptSource.Inline;
        public override ScriptSyntax Syntax { get; }

        public string GetScriptBody() => scriptBody;
    }
}
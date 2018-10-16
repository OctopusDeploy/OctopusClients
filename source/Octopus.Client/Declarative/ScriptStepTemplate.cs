using System;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.DeploymentProcess;
using Octopus.Client.Repositories.Async;
using Octopus.Client.Util;

namespace Octopus.Client.Declarative
{
    public class ScriptStepTemplate : DeclarativeResource
    {
        public string Name { get; set; }
        public ScriptTarget ScriptTarget { get; set; }
        public ScriptSyntax Syntax { get; set; }
        public string Body { get; set; }

        public string FromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
            var found = FileFinder.ResolveFileRelativeToType(fileName, GetType());
            if (found == null)
                throw new FileNotFoundException(fileName);

            return File.ReadAllText(found);
        }

        protected override async Task Apply(IOctopusAsyncRepository repository, IApplyContext context)
        {
            await CreateOrModify<IActionTemplateRepository, ActionTemplateResource>(context, repository.ActionTemplates, Name, template =>
            {
                template.Name = Name;
                template.ActionType = "Octopus.Script";
                template.Properties["Octopus.Action.Script.Syntax"] = Syntax.ToString();
                template.Properties["Octopus.Action.Script.RunOnServer"] = ScriptTarget == ScriptTarget.Server ? "true" : "false";
                template.Properties["Octopus.Action.Script.ScriptSource"] = "Inline";
                template.Properties["Octopus.Action.Script.ScriptBody"] = Body;
            });
        }
    }
}
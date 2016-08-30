using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Octopus.Client.Editors.DeploymentProcess;

namespace Octopus.Client.Model
{
    public class DeploymentStepResource
    {
        public string Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// This flag causes packages to be downloaded before the step runs regardless of whether any
        /// of the actions within the step need packages. If the actions need packages, then the step
        /// will be scheduled after acquisition regardless of the value of this flag.
        /// </summary>
        public bool RequiresPackagesToBeAcquired { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, PropertyValueResource> Properties { get; } = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);

        public DeploymentStepCondition Condition { get; set; }
        public DeploymentStepStartTrigger StartTrigger { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<DeploymentActionResource> Actions { get; } = new List<DeploymentActionResource>();

        public DeploymentStepResource ClearActions()
        {
            Actions.Clear();
            return this;
        }

        public DeploymentStepResource WithCondition(DeploymentStepCondition condition)
        {
            Condition = condition;
            return this;
        }

        public DeploymentStepResource WithStartTrigger(DeploymentStepStartTrigger startTrigger)
        {
            StartTrigger = startTrigger;
            return this;
        }

        public DeploymentStepResource RequirePackagesToBeAcquired(bool requirePackagesToBeAcquired = true)
        {
            RequiresPackagesToBeAcquired = requirePackagesToBeAcquired;
            return this;
        }

        public DeploymentStepResource TargetingRoles(params string[] roles)
        {
            var targetRoles = roles == null || roles.Length == 0
                ? null
                : string.Join(",", roles);

            Properties["Octopus.Action.TargetRoles"] = targetRoles;
            return this;
        }

        public DeploymentActionResource FindAction(string name)
        {
            return Actions.FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public DeploymentStepResource RemoveAction(string name)
        {
            Actions.Remove(FindAction(name));
            return this;
        }

        public DeploymentActionResource AddOrUpdateAction(string name)
        {
            var existing = FindAction(name);

            DeploymentActionResource action;
            if (existing == null)
            {
                action = new DeploymentActionResource
                {
                    Name = name
                };

                Actions.Add(action);
            }
            else
            {
                existing.Name = name;
                action = existing;
            }

            // Return the action so you can make customizations
            return action;
        }

        public DeploymentActionResource AddOrUpdateManualInterventionAction(string name, string instructions)
        {
            var action = AddOrUpdateAction(name);

            action.ActionType = "Octopus.Manual";
            action.Properties.Clear();
            action.Properties["Octopus.Action.Manual.Instructions"] = instructions;

            return action;
        }

        public DeploymentActionResource AddOrUpdateScriptAction(string name, ScriptAction scriptAction, ScriptTarget scriptTarget)
        {
            var action = AddOrUpdateAction(name);

            action.ActionType = "Octopus.Script";
            action.Properties.Clear();
            action.Properties["Octopus.Action.RunOnServer"] = scriptTarget == ScriptTarget.Server ? "true" : "false";
            action.Properties["Octopus.Action.Script.Syntax"] = scriptAction.Syntax.ToString();
            action.Properties["Octopus.Action.Script.ScriptSource"] = scriptAction.Source.ToString();

            RequiresPackagesToBeAcquired = scriptAction.Source == ScriptSource.Package;

            switch (scriptAction.Source)
            {
                case ScriptSource.Inline:
                    string scriptBody = null;
                    var inlineScript = scriptAction as InlineScriptAction;
                    if (inlineScript != null)
                    {
                        scriptBody = inlineScript.GetScriptBody();
                    }
                    var inlineScriptFromFileInAssembly = scriptAction as InlineScriptActionFromFileInAssembly;
                    if (inlineScriptFromFileInAssembly != null)
                    {
                        scriptBody = inlineScriptFromFileInAssembly.GetScriptBody();
                    }

                    if (scriptBody == null) throw new NotSupportedException($"{scriptAction.GetType().Name} is not a supported Script Action type yet...");

                    action.Properties.Add("Octopus.Action.Script.ScriptBody", scriptBody);
                    break;

                case ScriptSource.Package:
                    var packageScript = (ScriptActionFromFileInPackage) scriptAction;
                    action.Properties.Add("Octopus.Action.Package.PackageId", packageScript.PackageId);
                    action.Properties.Add("Octopus.Action.Package.FeedId", packageScript.PackageFeedId);
                    action.Properties.Add("Octopus.Action.Script.ScriptFileName", packageScript.ScriptFilePath);
                    break;
            }

            return action;
        }

        public DeploymentActionResource AddOrUpdatePackageAction(string name, PackageResource package)
        {
            var action = AddOrUpdateAction(name);

            action.ActionType = "Octopus.TentaclePackage";
            action.Properties["Octopus.Action.Package.PackageId"] = package.PackageId;
            action.Properties["Octopus.Action.Package.FeedId"] = package.FeedId;

            return action;
        }
    }
}
﻿using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class ChannelResource : Resource, INamedResource, IHaveSpaceResource, IHaveProject, IHaveSlugResource
    {
        public ChannelResource()
        {
            TenantTags = new ReferenceCollection();
            Rules = new List<ChannelVersionRuleResource>();
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        [WriteableOnCreate]
        public string ProjectId { get; set; }

        [Writeable]
        public string LifecycleId { get; set; }

        [Writeable]
        public bool IsDefault { get; set; }

        [Writeable]
        public List<ChannelVersionRuleResource> Rules { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        public ChannelResource SetAsDefaultChannel()
        {
            IsDefault = true;
            return this;
        }

        public ChannelResource UsingLifecycle(LifecycleResource lifecycle)
        {
            LifecycleId = lifecycle.Id;
            return this;
        }

        public ChannelResource ClearRules()
        {
            Rules.Clear();
            return this;
        }

        public ChannelResource AddRule(ChannelVersionRuleResource rule)
        {
            Rules.Add(rule);
            return this;
        }

        public ChannelResource AddCommonRuleForAllActions(string versionRange, string tagRegex, DeploymentProcessResource process)
        {
            var actionsWithPackage = process.Steps.SelectMany(s => s.Actions.Where(a => a.Properties.Any(p => p.Key == "Octopus.Action.Package.PackageId"))).ToArray();
            return AddRule(versionRange, tagRegex, actionsWithPackage);
        }

        /// <summary>
        /// Creates a rule for all packages used in the supplied actions 
        /// </summary>
        public ChannelResource AddRule(string versionRange, string tagRegex, params DeploymentActionResource[] actions)
        {
            Rules.Add(new ChannelVersionRuleResource
            {
                VersionRange = versionRange,
                Tag = tagRegex,
                ActionPackages = ( 
                    from action in actions 
                    from package in action.Packages
                    select new DeploymentActionPackageResource(action.Name, package.Name) 
                    ).ToList()
            });

            return this;
        }

        public ChannelResource ClearTenantTags()
        {
            TenantTags.Clear();
            return this;
        }

        public ChannelResource AddOrUpdateTenantTags(params TagResource[] tags)
        {
            foreach (var tag in tags)
            {
                TenantTags.Add(tag.CanonicalTagName);
            }

            return this;
        }

        public string SpaceId { get; set; }
        
        public string Slug { get; set; }
    }
}

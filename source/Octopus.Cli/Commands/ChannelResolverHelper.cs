using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Cli.Model;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public class ChannelResolverHelper : IChannelResolverHelper
    {
        readonly ILog log;
        IPackageVersionResolver versionResolver;

        public ChannelResolverHelper(ILog log, IPackageVersionResolver versionResolver)
        {
            this.log = log;
            this.versionResolver = versionResolver;
        }

        private IOctopusRepository _repository;
        private ProjectResource _project;
        IEnumerable<ChannelResource> _channels;

        public void SetContext(IOctopusRepository repository, ProjectResource project)
        {
            _repository = repository;
            _project = project;
            _channels = _repository.Projects.GetChannels(_project).Items;
        }

        private ReleaseTemplateResource GetReleaseTemplate(ChannelResource channel)
        {
            if (_repository != null && _project != null)
            {
                var deploymentProcess = _repository.DeploymentProcesses.Get(_project.DeploymentProcessId);
                return _repository.DeploymentProcesses.GetTemplate(deploymentProcess, channel);
            }

            return null;
        }

        private OctopusRuleTestResponse GetRuleTestResponse(ChannelResource channel, string packageVersion, string versionRange, string tag)
        {
            var checkChannelUri = string.Format("api/channels/rule-test?version={0}&versionRange={1}&preReleaseTag={2}", packageVersion, versionRange, tag);
            log.DebugFormat("Channel \"{0}\": Calling Octopus API to test rule => {1}", channel.Name, checkChannelUri);

            return _repository.Client.Get<OctopusRuleTestResponse>(checkChannelUri);
        }

        public IEnumerable<ChannelResource> GetChannels()
        {
            return _channels;
        }

        public int GetApplicableStepCount(ChannelResource channel)
        {
            // Need to generate the release plan for this channel, since steps can change depending on channel
            var plan = new ReleasePlan(GetReleaseTemplate(channel), versionResolver);

            // Steps from the release plan are the total number of steps we need to match
            return plan.Steps.Count;
        }

        public string ResolveVersion(ChannelResource channel, string step)
        {
            // Attempt to resolve by step name
            string version = versionResolver.ResolveVersion(step);
            if (version == null)
            {
                // Attempt to resolve with the package name for this step instead
                var package = GetPackageIdForStep(step, channel);
                version = versionResolver.ResolveVersion(package);
            }
            return version;
        }

        public string GetPackageIdForStep(string step, ChannelResource channel)
        {
            var package = GetReleaseTemplate(channel).Packages.FirstOrDefault(p => p.StepName == step);
            return package == null ? "" : package.NuGetPackageId;
        }

        public bool TestChannelRuleAgainstOctopusApi(ChannelResource channel, ChannelVersionRuleResource rule, string packageVersion)
        {
            var response = GetRuleTestResponse(channel, packageVersion, rule.VersionRange, rule.Tag);
            log.DebugFormat("Channel \"{0}\": API Response: SatisfiesVersionRang={1} SatisfiesPreReleaseTag={2}", channel.Name, response.SatisfiesVersionRange, response.SatisfiesPreReleaseTag);

            return response.SatisfiesVersionRange && response.SatisfiesPreReleaseTag;
        }
    }
}

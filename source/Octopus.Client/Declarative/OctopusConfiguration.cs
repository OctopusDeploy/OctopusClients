using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octopus.Client.Declarative
{
    public class OctopusConfiguration
    {
        readonly IList<IDeclarativeResource> resources = new List<IDeclarativeResource>();
        
        public OctopusConfiguration Ensure<T>() where T : IDeclarativeResource, new()
        {
            resources.Add(new T());
            return this;
        }

        public OctopusConfiguration Ensure<T>(T instance) where T : IDeclarativeResource
        {
            resources.Add(instance);
            return this;
        }
        
        public async Task<ConfigurationResult> Apply(IOctopusAsyncRepository repository)
        {
            var context = new ApplyContext {Action = ApplyAction.Commit};
            await ProcessChanges(repository, context);
            return new ConfigurationResult(context.Differences);
        }

        public async Task<ConfigurationResult> Test(IOctopusAsyncRepository repository)
        {
            var context = new ApplyContext { Action = ApplyAction.Detect };
            await ProcessChanges(repository, context);
            return new ConfigurationResult(context.Differences);
        }

        async Task ProcessChanges(IOctopusAsyncRepository repository, ApplyContext context)
        {
            foreach (var resource in resources)
            {
                await resource.Apply(repository, context);
            }
        }

        class ApplyContext : IApplyContext
        {
            public ApplyAction Action { get; set; }

            public List<Difference> Differences { get; } = new List<Difference>();

            public void ReportDifference(IDeclarativeResource resource, string reason)
            {
                Differences.Add(new Difference(reason));
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Client.Repositories.Async;
using Octopus.Client.Serialization;

namespace Octopus.Client.Declarative
{
    public abstract class DeclarativeResource : IDeclarativeResource
    {
        static readonly JsonSerializerSettings JsonSettings = JsonSerialization.GetDefaultSerializerSettings();

        Task IDeclarativeResource.Apply(IOctopusAsyncRepository repository, IApplyContext context)
        {
            return Apply(repository, context);
        }

        protected abstract Task Apply(IOctopusAsyncRepository repository, IApplyContext context);
        
        protected async Task CreateOrModify<R, T>(IApplyContext context, R collection, string name, Action<T> callback) where R : IFindByName<T>, ICreate<T>, IModify<T> where T : new()
        {
            var item = await collection.FindByName(name);
            if (item != null)
            {
                var serializer = JsonSerializer.Create(JsonSettings);
                var before = JToken.FromObject(item, serializer);

                callback(item);

                var after = JToken.FromObject(item, serializer);

                if (!JToken.DeepEquals(before, after))
                {
                    context.ReportDifference(this, "Modified");
                    await collection.Modify(item);
                }
            }
            else
            {
                context.ReportDifference(this, "New resource");

                item = new T();
                callback(item);
                await collection.Create(item);
            }
        }
    }
}
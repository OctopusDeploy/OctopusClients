using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Octopus.Server.MessageContracts.Base;
using Octopus.Server.MessageContracts.Base.HttpRoutes;
using HttpMethod = System.Net.Http.HttpMethod;

// ReSharper disable ReplaceWithSingleCallToSingleOrDefault
// ReSharper disable ReplaceWithSingleCallToSingle

namespace Octopus.Client.HttpRouting
{
    internal class HttpRouteExtractor : IHttpRouteExtractor
    {
        private static readonly Regex TokensRegex = new Regex("({.+?})",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private readonly ConcurrentDictionary<Type, HttpMethod> httpMethodsByPayloadType =
            new ConcurrentDictionary<Type, HttpMethod>();

        private readonly ConcurrentDictionary<Type, Type> httpRouteTypesByPayloadType =
            new ConcurrentDictionary<Type, Type>();

        private readonly Func<Type[]> scanForHttpRouteTypes;

        public HttpRouteExtractor(Func<Type[]> scanForHttpRouteTypes)
        {
            this.scanForHttpRouteTypes = scanForHttpRouteTypes;
        }

        public HttpMethod ExtractHttpMethod<TCommand, TResponse>(ICommand<TCommand, TResponse> command)
            where TCommand : ICommand<TCommand, TResponse>
            where TResponse : IResponse
        {
            var httpMethod = httpMethodsByPayloadType.GetOrAdd(typeof(TCommand), t =>
            {
                var httpRouteTypeInterface = typeof(IHttpCommandRouteFor<TCommand, TResponse>);
                var routeType = FindRouteType(httpRouteTypeInterface);
                var instance = (IHttpCommandRouteFor<TCommand, TResponse>) Activator.CreateInstance(routeType);
                return MapFromOctopusHttpMethodToSystemNetHttpMethod(instance.HttpMethod);
            });

            return httpMethod;
        }

        public HttpMethod ExtractHttpMethod<TRequest, TResponse>(IRequest<TRequest, TResponse> request)
            where TRequest : IRequest<TRequest, TResponse>
            where TResponse : IResponse
        {
            var httpMethod = httpMethodsByPayloadType.GetOrAdd(typeof(TRequest), t =>
            {
                var httpRouteTypeInterface = typeof(IHttpRequestRouteFor<TRequest, TResponse>);
                var routeType = FindRouteType(httpRouteTypeInterface);
                var instance = (IHttpRequestRouteFor<TRequest, TResponse>) Activator.CreateInstance(routeType);
                return MapFromOctopusHttpMethodToSystemNetHttpMethod(instance.HttpMethod);
            });

            return httpMethod;
        }

        public Uri ExtractHttpRoute<TCommand, TResponse>(ICommand<TCommand, TResponse> command)
            where TCommand : ICommand<TCommand, TResponse>
            where TResponse : IResponse
        {
            var httpRouteTypeInterface = typeof(IHttpCommandRouteFor<TCommand, TResponse>);
            var routeType = FindRouteType(httpRouteTypeInterface);
            var route = ExtractHttpRouteInternal(command, routeType);
            return route;
        }

        public Uri ExtractHttpRoute<TRequest, TResponse>(IRequest<TRequest, TResponse> request)
            where TRequest : IRequest<TRequest, TResponse>
            where TResponse : IResponse
        {
            var httpRouteTypeInterface = typeof(IHttpRequestRouteFor<TRequest, TResponse>);
            var routeType = FindRouteType(httpRouteTypeInterface);
            var route = ExtractHttpRouteInternal(request, routeType);
            return route;
        }

        private Uri ExtractHttpRouteInternal<T>(T payload, Type routeType)
        {
            var routeTemplatesAndTokens = routeType
                .GetFields()
                .Where(f => f.GetCustomAttributes(typeof(HttpRouteTemplateAttribute)).Any())
                .Select(f => (string) f.GetValue(payload))
                .ToDictionary(routeTemplate => routeTemplate,
                    routeTemplate =>
                    {
                        var matches = TokensRegex.Matches(routeTemplate);
                        var results = matches.OfType<Match>().Select(m => m.Value.Trim("{}".ToCharArray())).ToArray();
                        return results;
                    })
                .OrderByDescending(kvp => kvp.Value.Length)
                .ToArray();

            foreach (var kvp in routeTemplatesAndTokens)
            {
                if (TryReplaceAllRouteTokens(kvp.Key, kvp.Value, payload, out var route))
                    return new Uri(route, UriKind.Relative);
            }

            throw new Exception("Unable to resolve route template.");
        }

        private bool TryReplaceAllRouteTokens(string routeTemplate, string[] tokens, object payload,
            out string route)
        {
            var properties = payload.GetType().GetProperties();

            route = routeTemplate;
            foreach (var token in tokens)
            {
                var property = properties
                    .Where(p => p.Name.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                    .Single();
                var value = property.GetValue(payload);
                if (value is null) return false;

                route = route.Replace($"{{{token}}}", value.ToString());
            }

            return true;
        }

        private Type FindRouteType(Type httpRouteTypeInterface)
        {
            var routeType = httpRouteTypesByPayloadType.GetOrAdd(httpRouteTypeInterface,
                t => scanForHttpRouteTypes()
                    .Where(t.IsAssignableFrom)
                    .SingleOrDefault());

            return routeType;
        }

        private static HttpMethod MapFromOctopusHttpMethodToSystemNetHttpMethod(
            Server.MessageContracts.Base.HttpRoutes.HttpMethod httpMethod)
        {
            switch (httpMethod)
            {
                case Server.MessageContracts.Base.HttpRoutes.HttpMethod.Get:
                    return HttpMethod.Get;
                case Server.MessageContracts.Base.HttpRoutes.HttpMethod.Put:
                    return HttpMethod.Put;
                case Server.MessageContracts.Base.HttpRoutes.HttpMethod.Post:
                    return HttpMethod.Post;
                case Server.MessageContracts.Base.HttpRoutes.HttpMethod.Delete:
                    return HttpMethod.Delete;
                default:
                    throw new ArgumentOutOfRangeException(nameof(httpMethod), httpMethod, null);
            }
        }
    }
}
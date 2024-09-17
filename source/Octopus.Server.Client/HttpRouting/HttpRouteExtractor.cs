using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Octopus.Client.Extensions;
using Octopus.Server.MessageContracts.Base;
using Octopus.Server.MessageContracts.Base.HttpRoutes;
using HttpMethod = System.Net.Http.HttpMethod;

// ReSharper disable ReplaceWithSingleCallToSingleOrDefault
// ReSharper disable ReplaceWithSingleCallToSingle

namespace Octopus.Client.HttpRouting
{
    public class HttpRouteExtractor : IHttpRouteExtractor
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
            var httpMethod = ExtractHttpMethod(command);
            var httpRouteTypeInterface = typeof(IHttpCommandRouteFor<TCommand, TResponse>);
            var routeType = FindRouteType(httpRouteTypeInterface);
            var route = ExtractHttpRouteInternal(httpMethod, command, routeType);
            return route;
        }

        public Uri ExtractHttpRoute<TRequest, TResponse>(IRequest<TRequest, TResponse> request)
            where TRequest : IRequest<TRequest, TResponse>
            where TResponse : IResponse
        {
            var httpMethod = ExtractHttpMethod(request);
            var httpRouteTypeInterface = typeof(IHttpRequestRouteFor<TRequest, TResponse>);
            var routeType = FindRouteType(httpRouteTypeInterface);
            var route = ExtractHttpRouteInternal(httpMethod, request, routeType);
            return route;
        }

        private Uri ExtractHttpRouteInternal<T>(HttpMethod httpMethod, T payload, Type routeType)
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
                var routeTemplate = kvp.Key;
                var routePlaceholderTokens = kvp.Value;
                if (TryReplaceAllRouteTokens(httpMethod, routeTemplate, routePlaceholderTokens, payload, out var route))
                    return new Uri(route, UriKind.Relative);
            }

            throw new PayloadRoutingException("Unable to resolve route template.");
        }

        private bool TryReplaceAllRouteTokens(HttpMethod httpMethod, string routeTemplate, string[] tokens,
            object payload,
            out string route)
        {
            var payloadProperties = PayloadProperties(payload);

            route = routeTemplate;
            foreach (var token in tokens)
            {
                var payloadProperty = payloadProperties
                    .Where(kvp => kvp.Key.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();
                var key = payloadProperty.Key;
                var value = payloadProperty.Value;
                if (value is null) return false;

                route = route.Replace($"{{{token}}}", WebUtility.UrlEncode(value.ToString()));
                payloadProperties.Remove(key);
            }

            // If we're routing a GET request, every property not added to the route gets added as a query string
            // parameter - otherwise it can just go as a JSON payload.
            if (httpMethod == HttpMethod.Get)
            {
                var queryString = payloadProperties
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => $"{kvp.Key.ToCamelCase()}={WebUtility.UrlEncode(kvp.Value.ToString())}")
                    .StringJoin("&");

                if (!string.IsNullOrEmpty(queryString)) route += $"?{queryString}";
            }

            return true;
        }

        private static IDictionary<string, object> PayloadProperties(object payload)
        {
            var payloadProperties = payload
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes<ValidationAttribute>().Any()) // Only properties with validation attributes are candidates.
                .ToDictionary(p => p.Name, p => p.GetValue(payload))
                .Where(kvp => kvp.Value != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return payloadProperties;
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
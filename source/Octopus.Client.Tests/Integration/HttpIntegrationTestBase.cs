using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.IO;
using Nancy.ModelBinding;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Octopus.Client.Serialization;
using Nancy.Owin;
using Nancy.Responses.Negotiation;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using Nancy.Extensions;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Tests.Integration.Repository;
using HttpStatusCode = Nancy.HttpStatusCode;

namespace Octopus.Client.Tests.Integration
{
    public enum UrlPathPrefixBehaviour
    {
        UseClassNameAsUrlPathPrefix,
        UseNoPrefix
    }

    public abstract class HttpIntegrationTestBase : NancyModule
    {
        public static readonly int HostPort = 17358;
        public static readonly int HostSslPort = 17359;
        public static readonly string HostBaseUri = $"http://foo.localtest.me:{17358}";
        public static readonly string HostBaseSslUri = $"https://localhost:{17359}";
        protected static readonly Guid InstallationId = Guid.NewGuid();
        public static readonly byte[] SharedBytes = { 34, 56, 255, 0, 8 };
        static IWebHost currentHost;

        protected IOctopusAsyncClient AsyncClient { get; private set; }
#if SYNC_CLIENT
        protected IOctopusClient SyncClient { get; private set; }
#endif
        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            Console.WriteLine("HttpIntegrationTestBase OneTimeSetup");
            currentHost = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel(o =>
                    {
#if NET452
                        o.UseHttps(GetCert());
#else
                        o.Listen(IPAddress.Any, HostPort);
                        o.Listen(IPAddress.Any, HostSslPort, c => c.UseHttps(GetCert()));
#endif
                    }
                )
                .UseStartup<Startup>()
                .UseUrls(HostBaseUri, HostBaseSslUri)
                .Build();

            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("Running Host");
                    currentHost.Run();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            });
            var applicationLifetime = (IApplicationLifetime) currentHost.Services.GetService(typeof(IApplicationLifetime));
            applicationLifetime.ApplicationStarted.WaitHandle.WaitOne();
        }

        private static X509Certificate2 GetCert()
        {
            using (var s = typeof(HttpIntegrationTestBase).GetAssembly().GetManifestResourceStream($"{typeof(HttpIntegrationTestBase).Namespace}.TestingCert.pfx"))
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                return new X509Certificate2(ms.ToArray(), "password");
            }
        }


        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Console.WriteLine("HttpIntegrationTestBase OneTimeTearDown");
            currentHost?.Dispose();
        }

        protected HttpIntegrationTestBase(UrlPathPrefixBehaviour pathPrefixBehaviour)
        {
            TestRootPath = "/";
            if (pathPrefixBehaviour == UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
                TestRootPath = $"/{GetType().Name}";
            
            Get($"{TestRootPath}/api", p => Response.AsJson(
                 new RootResource()
                 {
                     ApiVersion = "3.0.0",
                     InstallationId = InstallationId,
                     Links = new LinkCollection()
                     {
                         { "CurrentUser",$"{TestRootPath}/api/users/me" },
                         { "SpaceHome", $"{TestRootPath}/api/{{spaceId}}" },
                         { "Users", $"{TestRootPath}/api/users/{{id}}" },
                     }
                 }
             ));
            Get($"{TestRootPath}/api/users/me", p => Response.AsJson(
                new UserResource()
                {
                    Links = new LinkCollection()
                    {
                        {"Spaces", TestRootPath + "/api/users/users-1/spaces" }
                    }
                }
            ));
            Get($"{TestRootPath}/api/users/users-1/spaces", p => Response.AsJson(
                new[] {
                    new SpaceResource() { Id = "Spaces-1", IsDefault = true},
                    new SpaceResource() { Id = "Spaces-2", IsDefault = false}
                }
            ));
            Get($"{TestRootPath}/api/spaces-1", p => Response.AsJson(
                new SpaceRootResource()
            ));
        }

        public string TestRootPath { get; }

        [SetUp]
        public async Task Setup()
        {
            AsyncClient = await Octopus.Client.OctopusAsyncClient.Create(new OctopusServerEndpoint(HostBaseUri + TestRootPath), GetClientOptions()).ConfigureAwait(false);
#if SYNC_CLIENT
            SyncClient = new Octopus.Client.OctopusClient(new OctopusServerEndpoint(HostBaseUri + TestRootPath));
#endif
        }

        protected virtual OctopusClientOptions GetClientOptions()
        {
            return new OctopusClientOptions();
        }

        public void TearDown()
        {
            AsyncClient?.Dispose();
#if SYNC_CLIENT
            SyncClient?.Dispose();
#endif
        }

        protected Response CreateErrorResponse(string message)
        {
            return Response.AsJson(new
            {
                ErrorMessage = message,
                Errors = new[] { message }
            }, HttpStatusCode.BadRequest);
        }

        protected bool CompareStreamToSharedBytes(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var bytes = ms.ToArray();
                if (bytes.Length != SharedBytes.Length)
                    return false;

                for (var x = 0; x < bytes.Length; x++)
                    if (bytes[x] != SharedBytes[x])
                        return false;
            }
            return true;
        }


        protected string GetCannedResponse(dynamic parameters)
        {
#if SYNC_CLIENT
            var assembly = typeof(HttpIntegrationTestBase).Assembly;
#else
            var assembly = typeof(HttpIntegrationTestBase).GetTypeInfo().Assembly;
#endif
            var resourceName = GetResourceNameFromtRequestUri(parameters);

            using (var responseStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (responseStream == null)
                {
                    var validResources = assembly.GetManifestResourceNames();
                    throw new Exception($"Didn't find a canned response '{resourceName}'.{Environment.NewLine}Valid resources names are: {Environment.NewLine}{string.Join(Environment.NewLine, validResources)}");
                }

                using (var reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private dynamic GetResourceNameFromtRequestUri(dynamic parameters)
        {
            var escapedUri = "/" + parameters.uri;
            foreach (var param in Request.Query.ToDictionary())
                escapedUri = escapedUri + "." + param.Key + "=" + param.Value;

            escapedUri = escapedUri
                .Replace("/api/", "")
                .Replace("/", ".")
                .Replace("?", ".")
                .Replace("&", ".")
                .Replace("-", "_");
            return $"Octopus.Client.Tests.CannedResponses.{escapedUri}.{Request.Method}.json";
        }

        public class Startup
        {
            public void Configure(IApplicationBuilder app)
            {
                app.UseOwin(x => x.UseNancy());
            }
        }

#region Nancy JSON Serializers
        public class JsonNetBodyDeserializer : IBodyDeserializer
        {
            private readonly JsonSerializer serializer;

            /// <summary>
            /// Empty constructor if no converters are needed
            /// </summary>
            public JsonNetBodyDeserializer()
            {
                this.serializer = JsonSerializer.CreateDefault();
            }

            /// <summary>
            /// Constructor to use when a custom serializer are needed.
            /// </summary>
            /// <param name="serializer">Json serializer used when deserializing.</param>
            public JsonNetBodyDeserializer(JsonSerializer serializer)
            {
                this.serializer = serializer;
            }

            /// <summary>
            /// Whether the deserializer can deserialize the content type
            /// </summary>
            /// <param name="mediaRange">Content type to deserialize</param>
            /// <param name="context">Current <see cref="BindingContext"/>.</param>
            /// <returns>True if supported, false otherwise</returns>
            public bool CanDeserialize(MediaRange mediaRange, BindingContext context)
            {
                return JsonNetSerializer.IsJsonType(mediaRange);
            }

            /// <summary>
            /// Deserialize the request body to a model
            /// </summary>
            /// <param name="mediaRange">Content type to deserialize</param>
            /// <param name="bodyStream">Request body stream</param>
            /// <param name="context">Current context</param>
            /// <returns>Model instance</returns>
            public object Deserialize(MediaRange mediaRange, Stream bodyStream, BindingContext context)
            {
                if (bodyStream.CanSeek)
                {
                    bodyStream.Position = 0;
                }

                var deserializedObject =
                    this.serializer.Deserialize(new StreamReader(bodyStream), context.DestinationType);

                var properties =
                    context.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(p => new BindingMemberInfo(p));

                var fields =
                    context.DestinationType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Select(f => new BindingMemberInfo(f));

                if (properties.Concat(fields).Except(context.ValidModelBindingMembers).Any())
                {
                    return CreateObjectWithBlacklistExcluded(context, deserializedObject);
                }

                return deserializedObject;
            }

            private static object ConvertCollection(object items, Type destinationType, BindingContext context)
            {
                var returnCollection = Activator.CreateInstance(destinationType);

                var collectionAddMethod =
                    destinationType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

                foreach (var item in (IEnumerable)items)
                {
                    collectionAddMethod.Invoke(returnCollection, new[] { item });
                }

                return returnCollection;
            }

            private static object CreateObjectWithBlacklistExcluded(BindingContext context, object deserializedObject)
            {
                var returnObject = Activator.CreateInstance(context.DestinationType, true);

                if (context.DestinationType.IsCollection())
                {
                    return ConvertCollection(deserializedObject, context.DestinationType, context);
                }

                foreach (var property in context.ValidModelBindingMembers)
                {
                    CopyPropertyValue(property, deserializedObject, returnObject);
                }

                return returnObject;
            }

            private static void CopyPropertyValue(BindingMemberInfo property, object sourceObject, object destinationObject)
            {
                property.SetValue(destinationObject, property.GetValue(sourceObject));
            }
        }

        // from https://github.com/NancyFx/Nancy.Serialization.JsonNet/tree/master/src/Nancy.Serialization.JsonNet
        public class JsonNetSerializer : ISerializer
        {
            private readonly JsonSerializer serializer;

            /// <summary>
            /// Initializes a new instance of the <see cref="JsonNetSerializer"/> class.
            /// </summary>
            public JsonNetSerializer()
            {
                this.serializer = JsonSerializer.CreateDefault(JsonSerialization.GetDefaultSerializerSettings());
            }


            /// <summary>
            /// Whether the serializer can serialize the content type
            /// </summary>
            /// <param name="mediaRange">Content type to serialise</param>
            /// <returns>True if supported, false otherwise</returns>
            public bool CanSerialize(MediaRange mediaRange)
            {
                return IsJsonType(mediaRange);
            }

            /// <summary>
            /// Gets the list of extensions that the serializer can handle.
            /// </summary>
            /// <value>An <see cref="IEnumerable{T}"/> of extensions if any are available, otherwise an empty enumerable.</value>
            public IEnumerable<string> Extensions
            {
                get { yield return "json"; }
            }

            /// <summary>
            /// Serialize the given model with the given contentType
            /// </summary>
            /// <param name="mediaRange">Content type to serialize into</param>
            /// <param name="model">Model to serialize</param>
            /// <param name="outputStream">Output stream to serialize to</param>
            /// <returns>Serialised object</returns>
            public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
            {
                using (var writer = new JsonTextWriter(new StreamWriter(new UnclosableStreamWrapper(outputStream))))
                {
                    this.serializer.Serialize(writer, model);
                }
            }

            public static bool IsJsonType(string contentType)
            {
                if (string.IsNullOrEmpty(contentType))
                {
                    return false;
                }

                var contentMimeType = contentType.Split(';')[0];

                return contentMimeType.Equals("application/json", StringComparison.OrdinalIgnoreCase) ||
                       contentMimeType.Equals("text/json", StringComparison.OrdinalIgnoreCase) ||
                      (contentMimeType.StartsWith("application/vnd", StringComparison.OrdinalIgnoreCase) &&
                       contentMimeType.EndsWith("+json", StringComparison.OrdinalIgnoreCase));
            }
        }

#endregion
    }
}

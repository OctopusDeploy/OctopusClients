using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.ModelBinding;
using Nancy.Owin;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Serilog;
using Octopus.Client.Serialization;

namespace Octopus.Cli.Tests.Integration
{

    public abstract class IntegrationTestBase : NancyModule
    {
        public static readonly string HostBaseUri = "http://localhost:18362";
        private static IWebHost _currentHost;

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            _currentHost = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls(HostBaseUri)
                .Build();
            Task.Run(() =>
            {
                try
                {
                    _currentHost.Run();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

            });
            var applicationLifetime = (IApplicationLifetime)_currentHost.Services.GetService(typeof(IApplicationLifetime));
            applicationLifetime.ApplicationStarted.WaitHandle.WaitOne();

        }


        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            _currentHost?.Dispose();
        }


        internal ExecuteResult Execute(string command, params string[] args)
        {
            var logOutput = new StringBuilder();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole(outputTemplate: "{Message}{NewLine}{Exception}")
                .WriteTo.TextWriter(new StringWriter(logOutput), outputTemplate: "{Message}{NewLine}{Exception}")
                .CreateLogger();

            var allArgs = new[]
                {
                    command,
                    $"--server={HostBaseUri}{TestRootPath}",
                    "--apiKey=ABCDEF123456789"
                }.Concat(args)
                .ToArray();

            var code = new CliProgram().Run(allArgs);
            return new ExecuteResult(code, logOutput.ToString());
        }


        protected IntegrationTestBase()
        {
            TestRootPath = $"/{GetType().Name}";
            Get($"{TestRootPath}/api", p => Response.AsJson(
                new RootResource()
                {
                    ApiVersion = "3.0.0",
                    Links = new LinkCollection()
                     {
                         { "CurrentUser", TestRootPath + "/api/users/me" },
                         { "Environments", TestRootPath + "/api/environments{/id}{?skip,ids}" },
                         { "PackageUpload", TestRootPath + "/api/packages/raw{?replace}" }
                     }
                }
            ));
            Get($"{TestRootPath}/api/users/me", p => Response.AsJson(
                new UserResource()
                {

                }
            ));
        }

        protected string TestRootPath { get; }

        internal class ExecuteResult
        {
            public int Code { get; }
            public string LogOutput { get; }

            public ExecuteResult(int code, string logOutput)
            {
                Code = code;
                LogOutput = logOutput;
            }
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

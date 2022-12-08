using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Serialization;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class WhenPostingFileToNonNancyEndpoint
    {
        public static readonly int HostPort = 17368;
        public static readonly int HostSslPort = 17369;
        public static readonly string HostBaseUri = $"http://foo.localtest.me:{HostPort}";
        public static readonly string HostBaseSslUri = $"https://localhost:{HostSslPort}";

        IWebHost webHost;
        IOctopusClient syncClient = new Octopus.Client.OctopusClient(new OctopusServerEndpoint($"{HostBaseUri}/"));

        [SetUp]
        public void Setup()
        {
            webHost = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel(o =>
                    {
                        o.Listen(IPAddress.Any, HostPort);
                        o.Listen(IPAddress.Any, HostSslPort, c => c.UseHttps(HttpIntegrationTestBase.GetCert()));
                    }
                )
                .ConfigureServices(services => services.AddRouting())
                .Configure(app =>
                {
                    app.UseOwin();
                    app.UseRouter(builder =>
                    {
                        builder.MapPost("FileTestsSync", async (request, response, data) =>
                        {
                            try
                            {
                                if (request.Form.Files.Count > 0)
                                    response.StatusCode = (int)HttpStatusCode.OK;
                            }
                            catch (Exception ex)
                            {
                                response.StatusCode = 400;
                                var error = new OctopusExceptionFactory.OctopusErrorsContract()
                                {
                                    ErrorMessage = ex.Message,
                                    Errors = Array.Empty<string>(),
                                    FullException = ex.ToString(),
                                };
                                
                                await response.WriteAsync(JsonSerialization.SerializeObject(error));
                            }
                        });

                        builder.Build();
                    });
                })
                .UseUrls(HostBaseUri, HostBaseSslUri)
                .Build();

            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("Running Host");
                    webHost.Run();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            });
        }

        [TearDown]
        public void Teardown()
        {
            syncClient?.Dispose();
            webHost?.Dispose();
        }

        [Test]
        public void WithSyncClient_ThenNoExceptionThrown()
        {
            using (var ms = new MemoryStream(HttpIntegrationTestBase.SharedBytes))
            {
                var file = new FileUpload
                {
                    Contents = ms,
                    FileName = "foo.txt"
                };

                syncClient.Post("FileTestsSync", file);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public abstract class OctopusClientTestBase : NancyModule
    {
        public static readonly Uri BaseUri = new Uri("http://localhost:17358");
        public static readonly byte[] SharedBytes = {34, 56, 255, 0, 8};
        private static NancyHost _currentHost;
        protected static Client.OctopusClient Client { get; private set; }
        public static Client.OctopusClient CurrentClient { get; set; }

        static OctopusClientTestBase()
        {
            AppDomainAssemblyTypeScanner.AssembliesToScan = new List<Func<Assembly, bool>>
            {
                x => x == typeof (NancyEngine).Assembly,
                x => x == typeof (OctopusClientTestBase).Assembly
            };
        }


        [OneTimeSetUp]
        public static void Setup()
        {
            _currentHost = new NancyHost(BaseUri);
            _currentHost.Start();
            Client = new Client.OctopusClient(new OctopusServerEndpoint(BaseUri.ToString()));
        }


        [OneTimeTearDown]
        public static void TearDown()
        {
            Client.Dispose();
            _currentHost.Dispose();
        }

        protected Response CreateErrorResponse(string message)
        {
            return Response.AsJson(new
            {
                ErrorMessage = message,
                Errors = new[] {message}
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
    }
}
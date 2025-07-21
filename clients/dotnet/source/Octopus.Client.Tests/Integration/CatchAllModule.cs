using System;
using Nancy;
using Serilog;

namespace Octopus.Client.Tests.Integration
{
    public class CatchAllModule : NancyModule
    {
        public CatchAllModule()
        {
            Get(@"/{uri*}", p =>
            {
                Console.Error.WriteLine($"Nothing listening at {Request.Url.Path}");
                return HttpStatusCode.NotFound;
            });
            Post(@"/{uri*}", p =>
            {
                Console.Error.WriteLine($"Nothing listening at {Request.Url.Path}");
                return HttpStatusCode.NotFound;
            });
        }
    }
}
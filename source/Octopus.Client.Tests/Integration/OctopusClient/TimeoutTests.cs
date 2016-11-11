using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Exceptions;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class TimeoutTests : HttpIntegrationTestBase
    {
        public TimeoutTests()
        {
            Get(TestRootPath, p =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                return "Foo";
            });
        }

        protected override OctopusClientOptions GetClientOptions()
        {
            var options = base.GetClientOptions();
            options.Timeout = TimeSpan.FromSeconds(1);
            return options;
        }

        [Test]
        public void ConfiguredTimeoutWorks()
        {
            var sw = Stopwatch.StartNew();
            Func<Task> get = () => Client.Get<string>("~/");
            get.ShouldThrow<TimeoutException>();
            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(4));
        }

    }
}
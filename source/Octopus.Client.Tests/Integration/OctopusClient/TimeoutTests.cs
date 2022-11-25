using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class TimeoutTests : HttpIntegrationTestBase
    {
        public TimeoutTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get(TestRootPath, p =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                return "Foo";
            });
        }

        protected override OctopusClientOptions GetClientOptions()
        {
            var options = base.GetClientOptions();
            options.Timeout = TimeSpan.FromSeconds(5);
            return options;
        }

        [Test]
        public void ConfiguredTimeoutWorks()
        {
            var sw = Stopwatch.StartNew();
            Func<Task> get = () => AsyncClient.Get<string>("~/");
            get.ShouldThrow<TimeoutException>();
            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        public void CancellationThrowsOperationCanceledException()
        {
            var sw = Stopwatch.StartNew();
            var cancellationTokenSource = new CancellationTokenSource();

            var getTask = AsyncClient.Get<string>("~/", cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();

            var get = () => getTask;
            get.ShouldThrow<OperationCanceledException>()
                .Where(ex => ex.CancellationToken == cancellationTokenSource.Token);

            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }
    }
}
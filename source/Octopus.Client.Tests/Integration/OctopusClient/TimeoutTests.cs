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
            var options = new OctopusClientOptions();
            options.Timeout = TimeSpan.FromSeconds(5);
            return options;
        }

        [Test]
        public async Task ConfiguredTimeoutWorks()
        {
            var sw = Stopwatch.StartNew();
            Func<Task> get = () => AsyncClient.Get<string>("~/");

            // We want a TimeoutException here, and we get one on .net 6.
            // The async/exception wrapping behaviour on .NET framework is such that we get the TaskCanceledException instead.
            // Functionally the timeout works fine, but doing extra work just to get the right kind of exception isn't worth
            // it for .NET framework
#if NETFRAMEWORK
            await get.Should().ThrowAsync<OperationCanceledException>();
#else
            await get.Should().ThrowAsync<TimeoutException>();
#endif
            
            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task CancellationThrowsOperationCanceledException()
        {
            var sw = Stopwatch.StartNew();
            var cancellationTokenSource = new CancellationTokenSource();

            var getTask = AsyncClient.Get<string>("~/", cancellationTokenSource.Token);
#if NETFRAMEWORK || NET6_0
            cancellationTokenSource.Cancel();
#else
            await cancellationTokenSource.CancelAsync();
#endif

            Func<Task> get = () => getTask;
            await get.Should().ThrowAsync<OperationCanceledException>();

            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10));
        }
    }
}
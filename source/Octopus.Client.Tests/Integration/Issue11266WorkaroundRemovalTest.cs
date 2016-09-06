#if COREFX_ISSUE_11456_EXISTS
using System;
using System.Net.Http;
using FluentAssertions;
using Nancy.Extensions;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration
{
    public class Issue11266WorkaroundRemovalTest
    {
        [Test]
        public void WhenTheNextVersionOf_IsReleasedIssue11266ShouldBeFixed()
        {
            typeof(WinHttpHandler).GetAssembly()
                .GetName()
                .Version.Should()
                .Be(new Version(4, 0, 0, 0),
                    "The Issue https://github.com/dotnet/corefx/issues/11266 should be fixed in the next release of System.Net.Http.WinHttpHandler, and the workaround (and this test) can be removed (See COREFX_ISSUE_11456_EXISTS compiler flag)");
        }
    }
}
#endif

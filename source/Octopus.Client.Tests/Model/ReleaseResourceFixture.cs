using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Model.Git;

namespace Octopus.Client.Tests.Model
{
    public class ReleaseResourceFixture
    {
        [Test]
        public void SetNullGitReference_ShouldGetNullVersionControlReference()
        {
            // Arrange
            var releaseResource = new ReleaseResource();

            // Act
            releaseResource.GitReference = null;

            // Act + Assert
            releaseResource.VersionControlReference.Should().BeNull();
        }

        [Test]
        public void SetValidGitReference_ShouldGetEquivalentVersionControlReference()
        {
            // Arrange
            var releaseResource = new ReleaseResource();

            var gitRef = "refs/heads/main";
            var gitCommit = "abcdef";

            // Act
            releaseResource.GitReference = new SnapshotGitReferenceResource
            {
                GitCommit = gitCommit,
                GitRef = gitRef
            };

            // Assert
            releaseResource.VersionControlReference.GitCommit.Should().Be(gitCommit);
            releaseResource.VersionControlReference.GitRef.Should().Be(gitRef);
        }

        [Test]
        public void ModifyGitCommitOnGitReference_ShouldModifyGitRefOnVersionControlReference()
        {
            // Arrange
            var releaseResource = new ReleaseResource();

            var gitRef = "refs/heads/main";
            var gitCommit = "abcdef";

            releaseResource.GitReference = new SnapshotGitReferenceResource
            {
                GitCommit = gitCommit,
                GitRef = gitRef
            };

            // Act
            var newGitCommit = "123456";
            releaseResource.GitReference.GitCommit = newGitCommit;

            // Assert
            releaseResource.VersionControlReference.GitCommit.Should().Be(newGitCommit);
        }

        [Test]
        public void SetNullVersionControlReference_ShouldGetNullGitReference()
        {
            // Arrange
            var releaseResource = new ReleaseResource();

            // Act
            releaseResource.VersionControlReference = null;

            // Assert
            releaseResource.GitReference.Should().BeNull();
        }

        [Test]
        public void SetValidVersionControlReference_ShouldGetEquivalentGitReference()
        {
            // Arrange
            var releaseResource = new ReleaseResource();

            var gitRef = "refs/heads/main";
            var gitCommit = "abcdef";

            // Act
            releaseResource.VersionControlReference = new GitReferenceResource
            {
                GitCommit = gitCommit,
                GitRef = gitRef
            };

            // Assert
            releaseResource.GitReference.GitRef.Should().Be(gitRef);
            releaseResource.GitReference.GitCommit.Should().Be(gitCommit);
            releaseResource.GitReference.VariablesGitCommit.Should().BeNull();
        }

        [Test]
        public void ModifyGitCommitOnVersionControlReference_ShouldModifyGitRefOnGitReference()
        {
            // Arrange
            var releaseResource = new ReleaseResource();

            var gitRef = "refs/heads/main";
            var gitCommit = "abcdef";

            releaseResource.VersionControlReference = new GitReferenceResource
            {
                GitCommit = gitCommit,
                GitRef = gitRef
            };

            // Act
            var newGitCommit = "123456";
            releaseResource.VersionControlReference.GitCommit = newGitCommit;

            // Assert
            releaseResource.GitReference.GitCommit.Should().Be(newGitCommit);
        }
    }
}
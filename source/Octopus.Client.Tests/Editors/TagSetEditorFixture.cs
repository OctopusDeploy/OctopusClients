using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using AsyncTagSetEditor = Octopus.Client.Editors.Async.TagSetEditor;
using AsyncTagSetRepository = Octopus.Client.Repositories.Async.ITagSetRepository;
using SyncTagSetEditor = Octopus.Client.Editors.TagSetEditor;
using SyncTagSetRepository = Octopus.Client.Repositories.ITagSetRepository;

namespace Octopus.Client.Tests.Editors
{
    [TestFixture]
    public class TagSetEditorFixture
    {
        const string Name = "Upgrade ring";
        const string NewName = "Upgrade rings";
        const string Description = "Which ring a tenant upgrades in";

        [Test]
        public async Task Async_CreateOrModify_WhenNoTagSetExists_CreatesItWithTheDescription()
        {
            var repository = Substitute.For<AsyncTagSetRepository>();
            repository.FindByName(Name, Arg.Any<CancellationToken>()).Returns((TagSetResource)null);
            repository.Create(Arg.Any<TagSetResource>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(ci.Arg<TagSetResource>()));

            var editor = await new AsyncTagSetEditor(repository)
                .CreateOrModify(Name, Description, CancellationToken.None);

            await repository.Received(1).Create(
                Arg.Is<TagSetResource>(t => t.Name == Name && t.Description == Description),
                Arg.Any<CancellationToken>());
            editor.Instance.Description.Should().Be(Description);
        }

        [Test]
        public async Task Async_CreateOrModify_WhenTheTagSetExists_AppliesTheNameAndTheDescription()
        {
            var existing = new TagSetResource { Name = Name, Description = "stale" };
            var repository = Substitute.For<AsyncTagSetRepository>();
            repository.FindByName(NewName, Arg.Any<CancellationToken>()).Returns(existing);
            repository.Modify(Arg.Any<TagSetResource>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult(ci.Arg<TagSetResource>()));

            var editor = await new AsyncTagSetEditor(repository)
                .CreateOrModify(NewName, Description, CancellationToken.None);

            await repository.Received(1).Modify(
                Arg.Is<TagSetResource>(t => t.Name == NewName && t.Description == Description),
                Arg.Any<CancellationToken>());
            editor.Instance.Name.Should().Be(NewName);
            editor.Instance.Description.Should().Be(Description);
        }

        [Test]
        public void Sync_CreateOrModify_WhenNoTagSetExists_CreatesItWithTheDescription()
        {
            var repository = Substitute.For<SyncTagSetRepository>();
            repository.FindByName(Name).Returns((TagSetResource)null);
            repository.Create(Arg.Any<TagSetResource>(), Arg.Any<object>())
                .Returns(ci => ci.Arg<TagSetResource>());

            var editor = new SyncTagSetEditor(repository).CreateOrModify(Name, Description);

            repository.Received(1).Create(
                Arg.Is<TagSetResource>(t => t.Name == Name && t.Description == Description),
                Arg.Any<object>());
            editor.Instance.Description.Should().Be(Description);
        }

        [Test]
        public void Sync_CreateOrModify_WhenTheTagSetExists_AppliesTheNameAndTheDescription()
        {
            var existing = new TagSetResource { Name = Name, Description = "stale" };
            var repository = Substitute.For<SyncTagSetRepository>();
            repository.FindByName(NewName).Returns(existing);
            repository.Modify(Arg.Any<TagSetResource>()).Returns(ci => ci.Arg<TagSetResource>());

            var editor = new SyncTagSetEditor(repository).CreateOrModify(NewName, Description);

            repository.Received(1).Modify(
                Arg.Is<TagSetResource>(t => t.Name == NewName && t.Description == Description));
            editor.Instance.Name.Should().Be(NewName);
            editor.Instance.Description.Should().Be(Description);
        }
    }
}

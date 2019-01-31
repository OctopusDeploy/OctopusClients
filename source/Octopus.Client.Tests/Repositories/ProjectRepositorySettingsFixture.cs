using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories
{
    [TestFixture]
    public class ProjectRepositorySettingsFixture
    {
        public class TestSettings
        {
            public string SomeSetting { get; set; }
        }

        [Test]
        public void ProjectSettingsAreDeserializedCorrectly()
        {
            var project = new ProjectResource
            {
                Name = "test",
                ExtensionSettings = new List<ExtensionSettingsValues>()
            };
            project.ExtensionSettings.Add(new ExtensionSettingsValues
            {
                ExtensionId = "test-id",
                Values = new TestSettings {  SomeSetting = "foo" }
            });

            var serialized = JsonConvert.SerializeObject(project);

            var deserializedObject = JsonConvert.DeserializeObject<ProjectResource>(serialized);

            var result = deserializedObject.GetExtensionSettings<TestSettings>("test-id");
            result.SomeSetting.Should().Be("foo");
        }
    }
}
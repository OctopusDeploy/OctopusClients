using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class ChannelVersionRuleResourceSerializationFixture 
    {
        [Test]
        public void ActionPackagesAreSerializedIntoLegacyActionsCollection()
        {
            var subject = new ChannelVersionRuleResource
            {
                Tag = "foo",
                ActionPackages = new List<DeploymentActionPackageResource>
                {
                    new DeploymentActionPackageResource("Action 1"),
                    new DeploymentActionPackageResource("Action 1", "Package 1"),
                    new DeploymentActionPackageResource("Action 2"),
                    new DeploymentActionPackageResource("Action 3", "Package 1"),
                }
            };

            var result = JObject.Parse(JsonSerialization.SerializeObject(subject));

            var expected = JObject.FromObject(new
            {
                VersionRange = (string)null,
                Tag = subject.Tag,
                ActionPackages = new object[]
                {
                    new {DeploymentAction = "Action 1", PackageReference = ""},
                    new {DeploymentAction = "Action 1", PackageReference = "Package 1"},
                    new {DeploymentAction = "Action 2", PackageReference = ""},
                    new {DeploymentAction = "Action 3", PackageReference = "Package 1"}
                },
                Links = new {},
                
                // This is the key part of this test: we are expecting the serialized object to contain an "Actions"
                // collection.  This maintains compatibility with older server 
                Actions = new[] {"Action 1", "Action 1:Package 1", "Action 2", "Action 3:Package 1"}
            });
            
            Assert.True(JToken.DeepEquals(expected, result));
        }

        [Test]
        public void LegacyActionsCollectionIsDeserializedIntoActionPackages_WhenTalkingToOlderServer()
        {
            var incoming = new
            {
                VersionRange = "[1.0]", 
                Actions = new[] {"Action 1", "Action 2", "Action 2:Package 1"}
            };

            var result =
                JsonSerialization.DeserializeObject<ChannelVersionRuleResource>(JObject.FromObject(incoming).ToString());
            
            Assert.AreEqual(incoming.VersionRange, result.VersionRange);
            Assert.AreEqual(3, result.ActionPackages.Count);
            
            Assert.True(result.ActionPackages.Any(ap => ap.DeploymentAction == "Action 1" && string.IsNullOrEmpty(ap.PackageReference)));
            Assert.True(result.ActionPackages.Any(ap => ap.DeploymentAction == "Action 2" && string.IsNullOrEmpty(ap.PackageReference)));
            Assert.True(result.ActionPackages.Any(ap => ap.DeploymentAction == "Action 2" && ap.PackageReference == "Package 1"));
        }
    }
}
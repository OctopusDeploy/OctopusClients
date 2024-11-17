using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Serialization;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class VersioningStrategyResourceSerializationFixture
    {
        [Test]
        [TestCase("Action 1", "", "Action 1")]
        [TestCase("Action 1", "Package 1", "Action 1:Package 1")]
        public void DonorPackageIsSerializedIntoLegacyDonorPackageStepId_ForPrimaryPackage(string deploymentActionName, 
            string packageReferenceName, string expectedDonorPackageStepId)
        {
            var subject = new VersioningStrategyResource
            {
                DonorPackage = new DeploymentActionPackageResource(deploymentActionName, packageReferenceName)
            };
            
            var result = JObject.Parse(JsonSerialization.SerializeObject(subject));

            var expected = JObject.FromObject(new
            {
                Template = (string) null,
                DonorPackage = new {DeploymentAction = deploymentActionName, PackageReference = packageReferenceName},
                DonorPackageStepId = expectedDonorPackageStepId 
            });
            
            Assert.True(JToken.DeepEquals(expected, result));
        }

        [Test]
        [TestCase("Action 1", "Action 1", null)]
        [TestCase("Action 1:Package 1", "Action 1", "Package 1")]
        public void LegacyDonorPackageStepIdIsDeserializedIntoDonorPackage_WhenTalkingToOlderServer(
            string donorPackageStepId, string expectedDeploymentAction, string expectedPackageReference)
        {
            var incoming = new
            {
                Template = (string)null,
                DonorPackageStepId = donorPackageStepId 
            };
            
            var result =
                JsonSerialization.DeserializeObject<VersioningStrategyResource>(JObject.FromObject(incoming).ToString());
            
            Assert.IsNull(result.Template);
            Assert.AreEqual(expectedDeploymentAction, result.DonorPackage.DeploymentAction);
            Assert.AreEqual(expectedPackageReference, result.DonorPackage.PackageReference);
        }
    }
}
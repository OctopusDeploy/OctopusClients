using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class PermissionSerializationFixture
    {
        private JsonSerializerSettings settings;

        [SetUp]
        public void SetUp()
        {
            settings = JsonSerialization.GetDefaultSerializerSettings();
            settings.Formatting = Formatting.None;
        }

        [Test]
        public void SerializesSinglePermissionCorrectly()
        {
            var testObject = new SinglePermissionTestStructure { Name = "Test", Permission = Permission.AdministerSystem };
            var serializedText = JsonConvert.SerializeObject(testObject, settings);
            Assert.AreEqual("{\"Name\":\"Test\",\"Permission\":\"AdministerSystem\"}", serializedText);
        }

        [Test]
        public void SerializesMultiplePermissionsCorrectly()
        {
            var testObject = new PermissionListTestStructure { Name = "Test", Permissions = new List<Permission> { Permission.AccountView, Permission.AccountEdit } };
            var serializedText = JsonConvert.SerializeObject(testObject, settings);
            Assert.AreEqual("{\"Name\":\"Test\",\"Permissions\":[\"AccountView\",\"AccountEdit\"]}", serializedText);
        }

        [Test]
        public void DeserializesSinglePermissionCorrectly()
        {
            var testObject = JsonConvert.DeserializeObject<SinglePermissionTestStructure>("{\"Name\":\"Test\",\"Permission\":\"AdministerSystem\"}", settings);
            Assert.IsNotNull(testObject);
            Assert.AreEqual(Permission.AdministerSystem, testObject.Permission);
        }

        [Test]
        public void DeserializesMultiplePermissionsCorrectly()
        {
            var testObject = JsonConvert.DeserializeObject<PermissionListTestStructure>("{\"Name\":\"Test\",\"Permissions\":[\"AccountView\",\"AccountEdit\"]}", settings);
            Assert.IsNotNull(testObject);
            CollectionAssert.AreEquivalent(new List<Permission> { Permission.AccountView, Permission.AccountEdit }, testObject.Permissions);
        }

        [Test]
        public void DeserializesPermissionsNotKnownToClientCorrectly()
        {
            var testObject = JsonConvert.DeserializeObject<PermissionListTestStructure>("{\"Name\":\"Test\",\"Permissions\":[\"AccountView\",\"AccountEdit\",\"SomeUnknownValue\"]}", settings);
            Assert.IsNotNull(testObject);
            CollectionAssert.AreEquivalent(new List<Permission> { Permission.AccountView, Permission.AccountEdit, new Permission("SomeUnknownValue") }, testObject.Permissions);
        }

        class SinglePermissionTestStructure
        {
            public string Name { get; set; }
            public Permission Permission { get; set; }
        }

        class PermissionListTestStructure
        {
            public string Name { get; set; }
            public List<Permission> Permissions { get; set; }
        }
    }
}
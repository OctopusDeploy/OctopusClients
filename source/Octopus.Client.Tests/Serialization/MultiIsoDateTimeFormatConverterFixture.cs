using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class MultiIsoDateTimeFormatConverterFixture
    {
        private const string PrimaryFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK";
        private const string SecondaryFormat = "dddd, dd MMMM yyyy h:mm tt zzz";
        readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            Converters = { 
                new MultiIsoDateTimeFormatConverter(PrimaryFormat, SecondaryFormat),
            },
        };

        static readonly DateTimeOffset selectedDate = new DateTimeOffset(2018, 11, 09, 21, 09, 00,  TimeSpan.FromHours(1));
        static readonly DummyDateClass testDateObj = new DummyDateClass() { Event = selectedDate };
        
        
        [Test]
        public void DateSerializesUsingDefaultFormat()
        {
            var raw = JsonConvert.SerializeObject(testDateObj, serializerSettings);
            StringAssert.Contains("2018-11-09T21:09:00.000+01:00", raw);
        }
        
        [Test]
        public void DeserializesBothFormats()
        {
            var deserialized1 = JsonConvert.DeserializeObject<DummyDateClass>("{\"Event\":\""+ selectedDate.ToString(PrimaryFormat) +"\"}", serializerSettings);
            var deserialized2 = JsonConvert.DeserializeObject<DummyDateClass>("{\"Event\":\""+ selectedDate.ToString(SecondaryFormat) +"\"}", serializerSettings);
            
            Assert.AreEqual(deserialized1.Event.Value.ToLocalTime(), deserialized2.Event.Value.ToLocalTime());
        }

        [Test]
        public void DeserializesNull()
        {
            var deserialized = JsonConvert.DeserializeObject<DummyDateClass>("{\"Event\":null}", serializerSettings);
            Assert.IsFalse(deserialized.Event.HasValue);
        }
        
        [Test]
        public void FailureToParseReturnsNull()
        {
            var deserialized = JsonConvert.DeserializeObject<DummyDateClass>("{\"Event\":\"FooDay, 09 Marching 2018 9:09 PM +01:00\"}", serializerSettings);
            Assert.IsFalse(deserialized.Event.HasValue);
        }
        
        class DummyDateClass{
            public DateTimeOffset? Event {get;set;}
        }
    }
}
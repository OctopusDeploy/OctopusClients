using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Octopus.Client.Serialization
{
    public class DateConverter : IsoDateTimeConverter
    {
        public DateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
using Newtonsoft.Json;
using System;
using MHLab.Patch.Core.Versioning;
using Version = MHLab.Patch.Core.Versioning.Version;

namespace MHLab.Patch.Utilities.Serializing
{
    public sealed class VersionConverter : JsonConverter<IVersion>
    {
        public override void WriteJson(JsonWriter writer, IVersion value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override IVersion ReadJson(JsonReader reader, Type objectType, IVersion existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var data = (string)reader.Value;
            return new Version(data);
        }
    }
}

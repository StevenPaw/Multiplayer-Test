using MHLab.Patch.Core.Serializing;
using Newtonsoft.Json;

namespace MHLab.Patch.Utilities.Serializing
{
    public sealed class NewtonsoftSerializer : ISerializer
    {
        public string Serialize<TObject>(TObject obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new VersionConverter());
        }

        public TObject Deserialize<TObject>(string data)
        {
            return JsonConvert.DeserializeObject<TObject>(data, new VersionConverter());
        }
    }
}

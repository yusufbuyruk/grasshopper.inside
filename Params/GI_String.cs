using Grasshopper.Kernel.Types;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class StringData : IData
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_String(Value);
    }
}

using Grasshopper.Kernel.Types;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class IntegerData : IData
    {
        [JsonProperty("min")]
        public int Min { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Integer(Value);
    }
}

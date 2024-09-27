using Grasshopper.Kernel.Types;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class NumberData : IData
    {
        [JsonProperty("min")]
        public float Min { get; set; }

        [JsonProperty("max")]
        public float Max { get; set; }

        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Number(Value);
    }
}

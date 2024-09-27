using Grasshopper.Kernel.Types;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class BooleanData : IData
    {
        [JsonProperty("value")]
        public bool Value { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Boolean(Value);
    }

}

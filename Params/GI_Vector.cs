using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class VectorData : IData
    {
        [JsonProperty("vector")]
        public float[] Vector { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Vector(new Vector3f(Vector[0], Vector[1], Vector[2]));
    }
}

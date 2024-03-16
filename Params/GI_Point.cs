using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class PointData : IData
    {
        [JsonProperty("point")]
        public float[] Point { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Point(new Point3f(Point[0], Point[1], Point[2]));
    }
}

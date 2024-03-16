using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class ArcGeom
    {
        [JsonProperty("plane")]
        public PlaneGeom Plane { get; set; }

        [JsonProperty("radius")]
        public float Radius { get; set; }

        [JsonProperty("angle")]
        public float AngleDegrees { get; set; }

        [JsonIgnore]
        public Arc RH_Arc
        {
            get
            {
                var plane = Plane.RH_Plane;
                var arc = new Arc(plane, Radius, RhinoMath.ToRadians(AngleDegrees));
                return arc;
            }
        }

        [JsonIgnore]
        public ArcCurve RH_ArcCurve => new ArcCurve(RH_Arc);

        public ArcGeom() { }
        public ArcGeom(Arc arc)
        {
            Plane = new PlaneGeom(arc.Plane);
            Radius = (float)arc.Radius;
            AngleDegrees = (float)arc.AngleDegrees;
        }
    }

    public class ArcData : IData
    {
        [JsonProperty("arc")]
        public ArcGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Arc(Geom.RH_Arc);
    }
}
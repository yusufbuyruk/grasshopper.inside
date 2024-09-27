using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;


namespace GrasshopperInside.Params
{
    public class CircleGeom
    {
        [JsonProperty("plane")]
        public PlaneGeom Plane { get; set; }
        
        [JsonProperty("radius")]
        public float Radius { get; set; }

        [JsonIgnore]
        public Circle RH_Circle
        {
            get
            {
                var plane = Plane.RH_Plane;
                var circle = new Circle(plane, Radius);
                return circle;
            }
        }

        public CircleGeom() { }
        public CircleGeom(Circle circle)
        {
            Plane = new PlaneGeom(circle.Plane);
            Radius = (float)circle.Radius;
        }
    }


    public class CircleData : IData
    {
        [JsonProperty("circle")]
        public CircleGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Circle(Geom.RH_Circle);
    }
}

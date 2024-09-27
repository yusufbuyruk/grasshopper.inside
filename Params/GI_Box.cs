using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class BoxGeom
    {
        [JsonProperty("plane")]
        public PlaneGeom Plane { get; set; }

        [JsonProperty("x")]
        public float[] X { get; set; }
        
        [JsonProperty("y")]
        public float[] Y { get; set; }

        [JsonProperty("z")]
        public float[] Z { get; set; }

        [JsonIgnore]
        public Box RH_Box
        {
            get
            {
                var xSize = new Interval(X[0], X[1]);
                var ySize = new Interval(Y[0], Y[1]);
                var zSize = new Interval(Z[0], Z[1]);
                var plane = Plane.RH_Plane;
                var box = new Box(plane, xSize, ySize, zSize);
                return box;
            }
        }

        public BoxGeom() { }
        public BoxGeom(Box box) 
        {
            Plane = new PlaneGeom(box.Plane);
            X = new float[] { (float)box.X.Min, (float)box.X.Max };
            Y = new float[] { (float)box.Y.Min, (float)box.Y.Max };
            Z = new float[] { (float)box.Z.Min, (float)box.Z.Max };
        }
    }

    public class BoxData : IData
    {
        [JsonProperty("box")]
        public BoxGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Box(Geom.RH_Box);
    }
}

using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class RectangleGeom
    {
        [JsonProperty("plane")]
        public PlaneGeom Plane { get; set; }

        [JsonProperty("x")]
        public float[] X { get; set; }

        [JsonProperty("y")]
        public float[] Y { get; set; }

        [JsonIgnore]
        public Rectangle3d RH_Rectangle
        {
            get
            {
                var plane = Plane.RH_Plane;
                var width = new Interval(X[0], X[1]);
                var height = new Interval(Y[0], Y[1]);
                var rectangle = new Rectangle3d(plane, width, height);
                return rectangle;
            }
        }

        public RectangleGeom() { }
        public RectangleGeom(Rectangle3d rectangle)
        {
            Plane = new PlaneGeom(rectangle.Plane);
            X = new float[] { (float)rectangle.X.Min, (float)rectangle.X.Max };
            Y = new float[] { (float)rectangle.Y.Min, (float)rectangle.Y.Max };
        }
    }

    public class RectangleData : IData
    {
        [JsonProperty("rectangle")]
        public RectangleGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Rectangle(Geom.RH_Rectangle);
    }
}

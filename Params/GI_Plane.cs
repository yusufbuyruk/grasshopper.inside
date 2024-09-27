using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;


namespace GrasshopperInside.Params
{
    public class PlaneGeom
    {
        [JsonProperty("origin")]
        public float[] Origin { get; set; }

        [JsonProperty("x_axis")]
        public float[] XAxis { get; set; }

        [JsonProperty("y_axis")]
        public float[] YAxis { get; set; }

        [JsonProperty("z_axis")]
        public float[] ZAxis { get; set; }

        [JsonIgnore]
        public Plane RH_Plane
        {
            get
            {
                var origin = new Point3d(Origin[0], Origin[1], Origin[2]);
                var x = new Vector3d(XAxis[0], XAxis[1], XAxis[2]);
                var y = new Vector3d(YAxis[0], YAxis[1], YAxis[2]);

                var plane = new Plane(origin, x, y);
                return plane;
            }
        }

        public PlaneGeom() { }
        public PlaneGeom(Plane plane)
        {
            Origin = new float[] { (float)plane.OriginX, (float)plane.OriginY, (float)plane.OriginZ };
            XAxis = new float[] { (float)plane.XAxis.X, (float)plane.XAxis.Y, (float)plane.XAxis.Z };
            YAxis = new float[] { (float)plane.YAxis.X, (float)plane.YAxis.Y, (float)plane.YAxis.Z };
            ZAxis = new float[] { (float)plane.ZAxis.X, (float)plane.ZAxis.Y, (float)plane.ZAxis.Z };
        }
    }

    public class PlaneData : IData
    {
        [JsonProperty("plane")]
        public PlaneGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Plane(Geom.RH_Plane);
    }
}

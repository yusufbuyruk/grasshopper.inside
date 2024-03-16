using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class LineGeom
    {
        [JsonProperty("start")]
        public float[] Start { get; set; }

        [JsonProperty("end")]
        public float[] End { get; set; }

        public LineGeom() { }
        public LineGeom(Line line)
        {
            Start = new float[] { (float)line.FromX, (float)line.FromY, (float)line.FromZ };
            End = new float[] { (float)line.ToX, (float)line.ToY, (float)line.ToZ };
        }

        [JsonIgnore]
        public Line RH_Line
        {
            get
            {
                var from = new Point3d(Start[0], Start[1], Start[2]);
                var to = new Point3d(End[0], End[1], End[2]);
                return new Line(from, to);
            }

        }

        [JsonIgnore]
        public LineCurve RH_LineCurve => new LineCurve(RH_Line);
    }

    public class LineData : IData
    {
        [JsonProperty("line")]
        public LineGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Line(Geom.RH_Line);
    }
}

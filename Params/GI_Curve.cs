using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Collections;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class NurbsCurveGeom
    {
        [JsonProperty("isperiodic")]
        public bool IsPeriodic { get; set; }

        [JsonProperty("degree")]
        public int Degree { get; set; }

        [JsonProperty("pointcount")]
        public int PointCount { get; set; }

        [JsonProperty("controlpoints")]
        public List<float[]> ControlPoints { get; set; }

        public NurbsCurveGeom() { }
        public NurbsCurveGeom(NurbsCurve nurbsCurve)
        {
            IsPeriodic = nurbsCurve.IsPeriodic;
            Degree = nurbsCurve.Degree;
            PointCount = nurbsCurve.Points.Count;

            ControlPoints = new List<float[]>();

            foreach (var point in nurbsCurve.Points)
                ControlPoints.Add(new float[] { (float)point.X, (float)point.Y, (float)point.Z, (float)point.Weight });
        }

        [JsonIgnore]
        public NurbsCurve RH_NurbsCurve
        {
            get
            {
                Point3dList points = new Point3dList(PointCount);
                foreach (var controlpoint in ControlPoints)
                    points.Add(new Point3d(controlpoint[0], controlpoint[1], controlpoint[2]));

                NurbsCurve nurbsCurve = NurbsCurve.Create(IsPeriodic, Degree, points);

                for (int i = 0; i < PointCount; i++)
                    nurbsCurve.Points.SetWeight(i, ControlPoints[i][3]);

                return nurbsCurve;
            }
        }
    }

    public class CurveGeom
    {
        [JsonProperty("nurbscurve", NullValueHandling = NullValueHandling.Ignore)]
        public List<NurbsCurveGeom> NurbsCurves { get; set; }

        [JsonProperty("arccurve", NullValueHandling = NullValueHandling.Ignore)]
        public List<ArcGeom> ArcCurves { get; set; }

        [JsonProperty("linecurve", NullValueHandling = NullValueHandling.Ignore)]
        public List<LineGeom> LineCurves { get; set; }

        public CurveGeom()
        {
            NurbsCurves = new List<NurbsCurveGeom>();
            ArcCurves = new List<ArcGeom>();
            LineCurves = new List<LineGeom>();
        }

        public CurveGeom(Curve curve)
        {
            NurbsCurves = new List<NurbsCurveGeom>();
            ArcCurves = new List<ArcGeom>();
            LineCurves = new List<LineGeom>();

            List<Curve> L = new List<Curve>();
            if (!Utils.CurveSegments(L, curve, true))
                throw new System.Exception("CurveSegments");

            foreach (var l in L)
            {
                if (l is NurbsCurve nurbsCurve)
                    NurbsCurves.Add(new NurbsCurveGeom(nurbsCurve));

                if (l is ArcCurve arcCurve)
                    ArcCurves.Add(new ArcGeom(arcCurve.Arc));

                if (l is LineCurve lineCurve)
                    LineCurves.Add(new LineGeom(lineCurve.Line));
            }
        }

        [JsonIgnore]
        public Curve RH_Curve
        {
            get
            {
                List<Curve> curveList = new List<Curve>();

                foreach (var nurbsCurve in NurbsCurves)
                    curveList.Add(nurbsCurve.RH_NurbsCurve);

                foreach (var arcCurve in ArcCurves)
                    curveList.Add(arcCurve.RH_ArcCurve);

                foreach (var lineCurve in LineCurves)
                    curveList.Add(lineCurve.RH_LineCurve);

                Curve[] crvs = Curve.JoinCurves(curveList);

                if (crvs.Length == 1) // if (crvs.Length == 1)
                    return crvs[0];
                else
                    return null;
            }
        }
    }

    public class CurveData : IData
    {
        [JsonProperty("curve")]
        public CurveGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data
        {
            get
            {
                if (Geom == null) return null;
                if (Geom.RH_Curve == null) return null;
                return new GH_Curve(Geom.RH_Curve);
            }
        }
    }
}
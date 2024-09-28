using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


public class ArcGeom
{
    public PlaneGeom plane;
    public float radius;
    public float angle;
}

public class ArcData : IData
{
    public ArcGeom arc;
}

public class BooleanData : IData
{
    public bool value;

    [JsonIgnore] public string label;
    [JsonIgnore] public bool oldValue;

    public override string ToString() => $"type: boolean, value: {value}";
}

public class BoxGeom
{
    public PlaneGeom plane;
    public float[] x;
    public float[] y;
    public float[] z;
}

public class BoxData : IData
{
    public BoxGeom box;
}

public class CircleGeom
{
    public PlaneGeom plane;
    public float radius;
}

public class CircleData : IData
{
    public CircleGeom circle;
}

public class NurbsCurveGeom
{
    public bool is_periodic;
    public int degree;
    public int point_count;
    public List<float[]> control_points;
}

public class CurveGeom
{
    public List<NurbsCurveGeom> nurbs_curves;
    public List<ArcGeom> arc_curves;
    public List<LineGeom> line_curves;
}

public class CurveData : IData
{
    public CurveGeom curve;
}

public class IntegerData : IData
{
    public int min;
    public int max;
    public int value;

    [JsonIgnore] public string label;
    [JsonIgnore] public int oldValue;

    public override string ToString() => $"type: integer, value: {value}, min: {min}, max: {max}";
}

public class LineGeom
{
    public float[] start;
    public float[] end;
}

public class LineData : IData
{
    public LineGeom line;
}

public class MeshGeom
{
    public List<float[]> vertices;
    public List<float[]> normals;
    public List<int[]> triangles;
    public List<int[]> quads;

    [JsonIgnore]
    public UnityEngine.Mesh UnityMesh
    {
        get
        {
            var unityVertices = new List<UnityEngine.Vector3>();
            var unityNormals = new List<UnityEngine.Vector3>();
            var unityTriangles = new List<int>();

            foreach (var vertex in vertices)
            {
                var unityVertex = new UnityEngine.Vector3(vertex[0] * 0.001f, vertex[2] * 0.001f, vertex[1] * 0.001f);
                unityVertices.Add(unityVertex);
            }

            foreach (var normal in normals)
            {
                var unityNormal = new UnityEngine.Vector3(normal[0], normal[2], normal[1]);
                unityNormals.Add(unityNormal);
            }

            foreach (var t in triangles)
            {
                unityTriangles.Add(t[0]);
                unityTriangles.Add(t[1]);
                unityTriangles.Add(t[2]);

                unityTriangles.Add(t[2]);
                unityTriangles.Add(t[1]);
                unityTriangles.Add(t[0]);
            }

            foreach (var q in quads)
            {
                unityTriangles.Add(q[0]);
                unityTriangles.Add(q[1]);
                unityTriangles.Add(q[2]);

                unityTriangles.Add(q[2]);
                unityTriangles.Add(q[1]);
                unityTriangles.Add(q[0]);

                unityTriangles.Add(q[2]);
                unityTriangles.Add(q[3]);
                unityTriangles.Add(q[0]);

                unityTriangles.Add(q[0]);
                unityTriangles.Add(q[3]);
                unityTriangles.Add(q[2]);
            }

            var mesh = new UnityEngine.Mesh
            {
                vertices = unityVertices.ToArray(),
                normals = unityNormals.ToArray(),
                triangles = unityTriangles.ToArray()
            };
            return mesh;
        }
    }
}

public class MeshData : IData
{
    public MeshGeom mesh;
}

public class NumberData : IData
{
    public float min;
    public float max;
    public float value;

    [JsonIgnore] public string label;
    [JsonIgnore] public float oldValue;

    public override string ToString() => $"type: number, value: {value}, min{min}, max{max}";
}

public class PlaneGeom
{
    public float[] origin;
    public float[] x_axis;
    public float[] y_axis;
    public float[] z_axis;
}

public class PlaneData : IData
{
    public PlaneGeom plane;
}

public class PointData : IData
{
    public float[] point;
}

public class RectangleGeom
{
    public PlaneGeom plane;
    public float[] x;
    public float[] y;
}

public class RectangleData : IData
{
    public RectangleGeom rectangle;
}

public class StringData : IData
{
    public string value;
}

public class SubDGeom
{
    public MeshGeom control_polygon;
}

public class SubDData : IData
{
    public SubDGeom subd;
}

public class SurfaceGeom
{
    //public SurfaceGeom(Rhino.Geometry.Surface surface)
    //{
    //    throw new Exception("Nurbs surface is not supported.");
    //}
}

public class SurfaceData : IData
{
    public SurfaceGeom surface;
}

public class VectorData : IData
{
    public float[] vector;
}



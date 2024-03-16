using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class MeshGeom
    {
        [JsonProperty("vertices")]
        public List<float[]> Vertices { get; set; }

        [JsonProperty("normals")]
        public List<float[]> Normals { get; set; }

        [JsonProperty("triangles")]
        public List<int[]> Triangles { get; set; }

        [JsonProperty("quads")]
        public List<int[]> Quads { get; set; }

        [JsonIgnore]
        public Mesh RH_Mesh
        {
            get
            {
                var mesh = new Mesh();

                foreach (var vertex in Vertices)
                    mesh.Vertices.Add(vertex[0], vertex[1], vertex[2]);

                foreach (var tri in Triangles)
                    mesh.Faces.AddFace(tri[0], tri[1], tri[2]);

                foreach (var quad in Quads)
                    mesh.Faces.AddFace(quad[0], quad[1], quad[2], quad[3]);

                foreach (var normal in Normals)
                    mesh.Normals.Add(normal[0], normal[1], normal[2]);

                // mesh.Normals.ComputeNormals(); // TODO: CHECK IF NECESSARY
                // mesh.Compact();

                return mesh;
            }
        }

        public MeshGeom() { }
        public MeshGeom(Mesh mesh)
        {
            Vertices = new List<float[]>();
            Normals = new List<float[]>();
            Triangles = new List<int[]>();
            Quads = new List<int[]>();
            
            foreach (var vertex in mesh.Vertices.ToPoint3fArray())
                Vertices.Add(new float[] { vertex.X, vertex.Y, vertex.Z });

            foreach (var normal in new List<Vector3f>(mesh.Normals))
                Normals.Add(new float[] { normal.X, normal.Y, normal.Z });

            foreach (var face in mesh.Faces)
            {
                if (face.IsTriangle)
                    Triangles.Add(new int[] { face.A, face.B, face.C });
                if (face.IsQuad)
                    Quads.Add(new int[] { face.A, face.B, face.C, face.D });
            }
        }
    }

    public class MeshData : IData
    {
        [JsonProperty("mesh")]
        public MeshGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_Mesh(Geom.RH_Mesh);   
    }
}


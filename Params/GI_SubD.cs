using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace GrasshopperInside.Params
{
    public class SubDGeom
    {
        [JsonProperty("control_polygon")]
        public MeshGeom ControlPolygon { get; set; }

        [JsonIgnore]
        public SubD RH_SubD
        {
            get
            {
                return SubD.CreateFromMesh(ControlPolygon.RH_Mesh);
                // SubD.CreateFromLoft(...);
                // SubD.CreateFromSweep(...);
            }
        }

        public SubDGeom() { }
        public SubDGeom(SubD subd)
        {
            var mesh = Mesh.CreateFromSubDControlNet(subd);
            ControlPolygon = new MeshGeom(mesh);
        }
    }

    public class SubDData : IData
    {
        [JsonProperty("subd")]
        public SubDGeom Geom { get; set; }

        [JsonIgnore]
        public object GH_Data => new GH_SubD(Geom.RH_SubD);
    }
}

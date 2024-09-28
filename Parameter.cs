using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI.Base;
using GrasshopperInside.Params;

using Rhino.Geometry;

namespace GrasshopperInside
{
    public interface IParam
    {
        int Index { get; set; }
        string Type { get; }
        string Label { get; }
    }

    public interface IInput : IParam
    {
        void SetData(string json);
    }

    public interface IOutput : IParam
    {
        void ComputeData();
    }

    public interface IData
    {
        object GH_Data { get; }
    }

    public class Parameter
    {
        private readonly IGH_Param _ighParam;
        private readonly string type;
        private readonly string label;
        private int index;

        [JsonIgnore]
        public Cluster Cluster { get; set; }

        [JsonIgnore]
        public IGH_Param Param => _ighParam;

        [JsonProperty("index", Order = -2)]
        public int Index { get => index; set { index = value; } }

        [JsonProperty("label", Order = -2)]
        public string Label => label;

        [JsonProperty("type", Order = -2)]
        public string Type => type;

        public Parameter() { }

        public Parameter(IGH_Param ighParam, Cluster cluster)
        {
            _ighParam = ighParam;
            label = _ighParam.NickName;
            type = _ighParam.TypeName;
            Cluster = cluster;
        }
    }

    public class Slider
    {
        [JsonProperty("min")]
        public int Min { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public class Input : Parameter, IInput
    {
        [JsonProperty("data")]
        public IData Data { get; set; }

        public Input() : base() { }
        public Input(IGH_Param ighParam, Cluster cluster) : base(ighParam, cluster)
        {
            switch (Param.TypeName)
            {
                case "Arc":
                    Data = new ArcData() { Geom = new ArcGeom() };
                    break;

                case "Boolean":
                    Data = new BooleanData();
                    break;

                case "Box":
                    Data = new BoxData() { Geom = new BoxGeom() };
                    break;

                case "Circle":
                    Data = new CircleData() { Geom = new CircleGeom() };
                    break;

                case "Curve":
                    Data = new CurveData() { Geom = new CurveGeom() };
                    break;

                case "Integer":
                    IntegerData integerData = new IntegerData() { Min = 0, Max = 100 };
                    if (ighParam.SourceCount > 0)
                        if (ighParam.Sources[0] is GH_NumberSlider numberSlider)
                            
                            // TODO: TEST
                            // if (numberSlider.Slider.Type != GH_SliderAccuracy.Float)
                            {
                                integerData.Min = (int)numberSlider.Slider.Minimum;
                                integerData.Max = (int)numberSlider.Slider.Maximum;
                                integerData.Value = (int)numberSlider.Slider.ValueF;
                            }
                    Data = integerData;
                    break;

                case "Line":
                    Data = new LineData() { Geom = new LineGeom() };
                    break;

                case "Mesh":
                    Data = new MeshData() { Geom = new MeshGeom() };
                    break;

                case "Number":
                    NumberData numberData = new NumberData() { Min = 0, Max = 100 };
                    if (ighParam.SourceCount > 0)
                        if (ighParam.Sources[0] is GH_NumberSlider numberSlider)

                            // TODO: TEST
                            // if (numberSlider.Slider.Type == GH_SliderAccuracy.Float)
                            {
                                numberData.Min = (float)numberSlider.Slider.Minimum;
                                numberData.Max = (float)numberSlider.Slider.Maximum;
                                numberData.Value = (float)numberSlider.Slider.ValueF;
                            }
                    Data = numberData;
                    break;

                case "Plane":
                    Data = new PlaneData() { Geom = new PlaneGeom() };
                    break;

                case "Point":
                    Data = new PointData() { Point = new float[3] };
                    break;

                case "Rectangle":
                    Data = new RectangleData() { Geom = new RectangleGeom() };
                    break;

                case "Text":
                    Data = new StringData() { Value = string.Empty };
                    break;

                case "SubD":
                    Data = new SubDData() { Geom = new SubDGeom() };
                    break;

                case "Surface":
                    // Data = new SurfaceData() { Geom = new SurfaceGeom() };
                    break;

                case "Vector":
                    Data = new VectorData() { Vector = new float[3] }; break;
            }
        }

        public void SetData(string json)
        {
            switch (Param.TypeName)
            {
                case "Arc":
                    Data = JsonConvert.DeserializeObject<ArcData>(json);
                    break;

                case "Boolean":
                    Data = JsonConvert.DeserializeObject<BooleanData>(json);
                    break;

                case "Box":
                    Data = JsonConvert.DeserializeObject<BoxData>(json);
                    break;

                case "Circle":
                    Data = JsonConvert.DeserializeObject<CircleData>(json);
                    break;

                case "Curve":
                    Data = JsonConvert.DeserializeObject<CurveData>(json);
                    break;

                case "Integer":
                    Data = JsonConvert.DeserializeObject<IntegerData>(json);
                    break;

                case "Line":
                    Data = JsonConvert.DeserializeObject<LineData>(json);
                    break;

                case "Mesh":
                    Data = JsonConvert.DeserializeObject<MeshData>(json);
                    break;

                case "Number":
                    Data = JsonConvert.DeserializeObject<NumberData>(json);
                    break;

                case "Plane":
                    Data = JsonConvert.DeserializeObject<PlaneData>(json);
                    break;

                case "Point":
                    Data = JsonConvert.DeserializeObject<PointData>(json);
                    break;

                case "Rectangle":
                    Data = JsonConvert.DeserializeObject<RectangleData>(json);
                    break;

                case "Text":
                    Data = JsonConvert.DeserializeObject<StringData>(json);
                    break;

                case "SubD":
                    Data = JsonConvert.DeserializeObject<SubDData>(json);
                    break;

                case "Surface":
                    // Data = JsonConvert.DeserializeObject<SurfaceData>(json);
                    break;

                case "Vector":
                    Data = JsonConvert.DeserializeObject<VectorData>(json);
                    break;

                default:
                    throw new System.Exception("Type not supported");
            }


            Param.VolatileData.Clear();
            Param.ExpireSolution(false);

            if (Data != null)
                Param.AddVolatileData(new GH_Path(0), 0, Data.GH_Data);
        }
    }

    public class Output : Parameter, IOutput
    {
        [JsonProperty("data")]
        public List<IData> Data { get; set; }

        public Output() : base() { }
        public Output(IGH_Param ighParam, Cluster cluster) : base(ighParam, cluster)
        {
            Data = new List<IData>();
        }

        public void ComputeData()
        {
            Data.Clear();

            Param.CollectData();
            Param.ComputeData();


            // TODO SEARCH : Param.Kind vs Param.TypeName
            // TODO SEARCH : Param.Phase = GH_SolutionPhase.Collecting | GH_SolutionPhase.Collected | GH_SolutionPhase.Computed;

            // var item = (GH_<Data>)Param.VolatileData.get_Branch(0)[0];
            // Data = item.Value;

            foreach (var item in Param.VolatileData.AllData(true))
            {
                switch (Param.TypeName)
                {
                    case "Arc":
                        if (item.CastTo(out Arc arc))
                            Data.Add(new ArcData() { Geom = new ArcGeom(arc) });
                        break;

                    case "Boolean":
                        if (item.CastTo(out bool b))
                            Data.Add(new BooleanData() { Value = b });
                        break;

                    case "Box":
                        if (item.CastTo(out Box box))
                            Data.Add(new BoxData() { Geom = new BoxGeom(box) });
                        break;

                    case "Circle":
                        if (item.CastTo(out Circle circle))
                            Data.Add(new CircleData() { Geom = new CircleGeom(circle) });
                        break;

                    case "Curve":
                        if (item.CastTo(out Curve curve))
                            Data.Add(new CurveData() { Geom = new CurveGeom(curve) });
                        break;

                    case "Integer":
                        if (item.CastTo(out int i))
                            Data.Add(new IntegerData() { Value = i });
                        break;

                    case "Line":
                        if (item.CastTo(out Line line))
                            Data.Add(new LineData() { Geom = new LineGeom(line) });
                        break;

                    case "Mesh":
                        if (item.CastTo(out Mesh mesh))
                            if (Label != "static")
                                Data.Add(new MeshData() { Geom = new MeshGeom(mesh) });
                        break;

                    case "Number":
                        if (item.CastTo(out float f))
                            Data.Add(new NumberData() { Value = f });
                        break;

                    case "Plane":
                        if (item.CastTo(out Plane plane))
                            Data.Add(new PlaneData() { Geom = new PlaneGeom(plane) });
                        break;

                    case "Point":
                        if (item.CastTo(out Point3f point))
                            Data.Add(new PointData() { Point = new float[] { point.X, point.Y, point.Z } });
                        break;

                    case "Rectangle":
                        if (item.CastTo(out Rectangle3d rectangle))
                            Data.Add(new RectangleData() { Geom = new RectangleGeom(rectangle) });
                        break;

                    case "Text":
                        if (item.CastTo(out string s))
                            Data.Add(new StringData() { Value = s });
                        break;

                    case "SubD":
                        if (item.CastTo(out SubD subd))
                            Data.Add(new SubDData() { Geom = new SubDGeom(subd) });
                        break;

                    case "Surface":
                        //if (item.CastTo(out Surface surface))
                        //    Data.Add(new SurfaceData() { Geom = new SurfaceGeom(surface) });
                        break;

                    case "Vector":
                        if (item.CastTo(out Vector3f vector))
                            Data.Add(new VectorData() { Vector = new float[] { vector.X, vector.Y, vector.Z } });
                        break;
                }
            }
        }
    }
}


using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


public interface IParam 
{ 
    int index { get; set; } 
    string type { get; set; } 
    string label { get; set; }
}

public interface IInput : IParam 
{ 
    IData data { get; set; } 
}

public interface IOutput : IParam 
{ 
    List<IData> data { get; set; } 
}

public interface IData { }

public class GI_Param : IParam
{
    [JsonProperty(Order = -2)]
    public int index { get; set; }

    [JsonProperty(Order = -2)]
    public string type { get; set; }

    [JsonProperty(Order = -2)]
    public string label { get; set; }
}

public class GI_Input : GI_Param, IInput
{
    public IData data { get; set; }

    public GI_Input(JToken jInput)
    {
        type = jInput["type"].ToString();
        label = jInput["label"].ToString();
        index = (int)jInput["index"];
        var jsonData = jInput["data"].ToString();

        switch (type)
        {
            case "Arc": data = JsonConvert.DeserializeObject<ArcData>(jsonData); break;
            case "Boolean": data = JsonConvert.DeserializeObject<BooleanData>(jsonData); break;
            case "Box": data = JsonConvert.DeserializeObject<BoxData>(jsonData); break;
            case "Circle": data = JsonConvert.DeserializeObject<CircleData>(jsonData); break;
            case "Curve": data = JsonConvert.DeserializeObject<CurveData>(jsonData); break;
            case "Integer": data =  JsonConvert.DeserializeObject<IntegerData>(jsonData); break;
            case "Line": data =  JsonConvert.DeserializeObject<LineData>(jsonData); break;
            case "Mesh": data = JsonConvert.DeserializeObject<MeshData>(jsonData); break;
            case "Number": data =  JsonConvert.DeserializeObject<NumberData>(jsonData); break;
            case "Plane": data =  JsonConvert.DeserializeObject<PlaneData>(jsonData); break;
            case "Point": data =  JsonConvert.DeserializeObject<PointData>(jsonData); break;
            case "Rectangle": data = JsonConvert.DeserializeObject<RectangleData>(jsonData); break;
            case "Text": data =  JsonConvert.DeserializeObject<StringData>(jsonData); break; 
            case "SubD": data =  JsonConvert.DeserializeObject<SubDData>(jsonData); break;
            case "Surface": throw new System.Exception("Surface not implemented, use meshes instead...");
            case "Vector": data = JsonConvert.DeserializeObject<VectorData>(jsonData); break;
            default: throw new System.Exception("Type not supported");
        }
    }
}

public class GI_Output : GI_Param, IOutput
{
    public List<IData> data { get; set; }

    public GI_Output(JToken jOutput)
    {
        data = new List<IData>();

        type = jOutput["type"].ToString();
        label = jOutput["label"].ToString();
        index = (int)jOutput["index"];
        var jDataList = jOutput["data"].Children();

        foreach (var jData in jDataList)
        {
            var jsonData = jData.ToString();

            switch (type)
            {
                case "Arc": data.Add(JsonConvert.DeserializeObject<ArcData>(jsonData)); break;
                case "Boolean": data.Add(JsonConvert.DeserializeObject<BooleanData>(jsonData)); break;
                case "Box": data.Add(JsonConvert.DeserializeObject<BoxData>(jsonData)); break;
                case "Circle": data.Add(JsonConvert.DeserializeObject<CircleData>(jsonData)); break;
                case "Curve": data.Add(JsonConvert.DeserializeObject<CurveData>(jsonData)); break;
                case "Integer": data.Add(JsonConvert.DeserializeObject<IntegerData>(jsonData)); break;
                case "Line": data.Add(JsonConvert.DeserializeObject<LineData>(jsonData)); break;
                case "Mesh": data.Add(JsonConvert.DeserializeObject<MeshData>(jsonData)); break;
                case "Number": data.Add(JsonConvert.DeserializeObject<NumberData>(jsonData)); break;
                case "Plane": data.Add(JsonConvert.DeserializeObject<PlaneData>(jsonData)); break;
                case "Point": data.Add(JsonConvert.DeserializeObject<PointData>(jsonData)); break;
                case "Rectangle": data.Add(JsonConvert.DeserializeObject<RectangleData>(jsonData)); break;
                case "Text": data.Add(JsonConvert.DeserializeObject<StringData>(jsonData)); break;
                case "SubD": data.Add(JsonConvert.DeserializeObject<SubDData>(jsonData)); break;
                case "Surface": throw new System.Exception("Surface not implemented, use meshes instead...");
                case "Vector": data.Add(JsonConvert.DeserializeObject<VectorData>(jsonData)); break;
                default: throw new System.Exception("Type not supported");
            }
        }
    }
}
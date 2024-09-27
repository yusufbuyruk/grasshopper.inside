using System.Collections.Generic;
using Newtonsoft.Json.Linq;


public class GI_Cluster
{
    public string guid;
    public string label;
    
    public List<IInput> inputs = new();
    public List<IOutput> outputs = new();

    public GI_Cluster(string json)
    {
        JObject jCluster = JObject.Parse(json);

        var jInputs = jCluster["inputs"].Children();
        var jOutputs = jCluster["outputs"].Children();

        guid = jCluster["guid"].ToString();
        label = jCluster["label"].ToString();

        foreach (var jInput in jInputs)
            inputs.Add(new GI_Input(jInput));

        foreach (var jOutput in jOutputs)
            outputs.Add(new GI_Output(jOutput));
    }
}


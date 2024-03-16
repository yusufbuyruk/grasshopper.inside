using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GrasshopperInside.Params;

using Newtonsoft.Json;

using System;
using Newtonsoft.Json.Linq;

namespace GrasshopperInside
{
    public class Cluster
    {
        public readonly string guid;
        private readonly GH_Cluster _ghCluster;

        [JsonProperty("label")]
        public readonly string Label;

        [JsonProperty("inputs")]
        public List<IInput> Inputs { get; set; }

        [JsonProperty("outputs")]
        public List<IOutput> Outputs { get; set; }

        public Cluster() { }
        public Cluster(GH_Cluster cluster)
        {
            _ghCluster = cluster;

            Label = cluster.NickName;     // cluster.Name
            Inputs = new List<IInput>();
            Outputs = new List<IOutput>();

            guid = cluster.InstanceGuid.ToString();

            foreach (var input in cluster.Params.Input)
                Inputs.Add(new Input(input, this));
            
            foreach (var output in cluster.Params.Output)
                Outputs.Add(new Output(output, this));

            for (int i = 0; i < Inputs.Count; i++)
                Inputs[i].Index = i;

            for (int j = 0; j < Outputs.Count; j++)
                Outputs[j].Index = j;
        }

        public void SetInputs(string json)
        {
            var jCluster = JObject.Parse(json);
            var jInputs = (JArray)jCluster["inputs"];

            if (Inputs.Count != jInputs.Count)
                throw new Exception("Input Count Mismatch Error");

            foreach (var jInput in jInputs)
            {
                var type = jInput["type"].ToString();
                var index = (int)jInput["index"];
                var data = jInput["data"].ToString();
                var label = jInput["label"].ToString();

                var giInput = Inputs[index];

                if (type != giInput.Type.ToString())
                    throw new Exception("Type Mismatch Error");

                if (label != giInput.Label)
                    throw new Exception("Label Mismatch Error");

                // throw new Exception("Input Order Mismatch Error");

                giInput.SetData(data);
            }
        }

        public void ComputeOutputs()
        {
            foreach (var output in Outputs)
                output.ComputeData();
        }
    }
}
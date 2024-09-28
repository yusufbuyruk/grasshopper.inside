using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.Events;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GI_Component : MonoBehaviour
{

    [HideInInspector] public string url = "http://localhost";
    [HideInInspector] public int port = 4444;

    [HideInInspector] public int documentId = 0;
    [HideInInspector] public int clusterId = 0;

    [HideInInspector] public string documentName;
    [HideInInspector] public string clusterName;

    public Material mat;

    private GI_Cluster requestCluster;
    private GI_Cluster responseCluster;

    private readonly List<NumberData> numberParameters = new();
    private readonly List<IntegerData> integerParameters = new();
    private readonly List<BooleanData> booleanParameters = new();

    private List<GameObject> goo = new();


    private bool isCoroutineRunning = false;

    private Coroutine coroutine = null;

    public void SetConnectionInfo(string url, int port, int documentId, int clusterId)
    {
        this.url = url;
        this.port = port;
        this.documentId = documentId;
        this.clusterId = clusterId;
    }



    public void Init(string json)
    {
        Log(json);

        requestCluster = new GI_Cluster(json);

        foreach (var input in requestCluster.inputs)
        {
            if (input.data is NumberData numberData)
            {
                numberData.label = input.label;
                numberParameters.Add(numberData);
                Debug.Log($"NumberData {numberData}");
            }

            if (input.data is IntegerData integerData)
            {
                integerData.label = input.label;
                integerParameters.Add(integerData);
                Debug.Log($"IntegerData {integerData}");
            }

            if (input.data is BooleanData booleanData)
            {
                booleanData.label = input.label;
                booleanParameters.Add(booleanData);
                Debug.Log($"BooleanData {booleanData}");
            }
        }

        isCoroutineRunning = true;
        coroutine = StartCoroutine(ComputeAsync());

    }

    void Start()
    {
        _ = StartCoroutine(GetRequest($"api/getcluster/{clusterId}", Init));
    }

    public void LoadDocument()
    {
        _ = StartCoroutine(GetRequest($"api/load/{documentId}", Log));
    }


    void OnGUI()
    {
        // GUI.BeginGroup(new Rect(25, 25, 400, 600));

        int space = -25;

        // Update parameters to dynamically generate meshes
        // bool valueChanged = false;

        foreach (var booleanParameter in booleanParameters)
        {
            space += 50;

            GUI.Box(new Rect(20, space, 400, 45), "");
            GUI.Label(new Rect(35, 15 + space, 100, 25), booleanParameter.label);
            booleanParameter.value = GUI.Toggle(new Rect(150, 15 + space, 30, 30), booleanParameter.value, "");

            //  Update parameters to dynamically generate meshes
            //  if (booleanParameter.value != booleanParameter.oldValue) valueChanged = true;
        }

        foreach (var integerParameter in integerParameters)
        {
            space += 50;

            GUI.Box(new Rect(20, space, 400, 45), "");
            GUI.Label(new Rect(35, 15 + space, 100, 25), integerParameter.label);
            integerParameter.value = (int)GUI.HorizontalSlider(new Rect(150, 20 + space, 200, 30), integerParameter.value, integerParameter.min, integerParameter.max);
            GUI.Label(new Rect(360, 15 + space, 100, 25), $"{integerParameter.value}");

            //  Update parameters to dynamically generate meshes
            //  if (integerParameter.value != integerParameter.oldValue) valueChanged = true;

        }

        foreach (var numberParameter in numberParameters)
        {
            space += 50;

            GUI.Box(new Rect(20, space, 400, 45), "");
            GUI.Label(new Rect(35, 15 + space, 100, 25), numberParameter.label);
            numberParameter.value = GUI.HorizontalSlider(new Rect(150, 20 + space, 200, 30), numberParameter.value, numberParameter.min, numberParameter.max);
            GUI.Label(new Rect(360, 15 + space, 100, 25), $"{numberParameter.value:0.00}");

            //  Update parameters to dynamically generate meshes
            //  if (numberParameter.value != numberParameter.oldValue) valueChanged = true;
        }



        space += 50;
        if (GUI.Button(new Rect(20, 10 + space, 400, 45), "Compute"))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                isCoroutineRunning = false;
            }

            isCoroutineRunning = true;
            coroutine = StartCoroutine(ComputeAsync());
        }

        /*
        //
        // Update parameters to dynamically generate meshes without requiring the compute button to be pressed
        //
        
        if (valueChanged && !isCoroutineRunning)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                isCoroutineRunning = false;
            }

            isCoroutineRunning = true;
            coroutine = StartCoroutine(ComputeAsync());
            
            UpdateParameterValues();
        }
        */



        // GUI.EndGroup();
    }

    IEnumerator ComputeAsync()
    {
        // yield return new WaitForSeconds(0.5f);

        yield return Compute();

        coroutine = null;
        isCoroutineRunning = false;
    }

    IEnumerator Compute()
    {
        string json = JsonConvert.SerializeObject(requestCluster, Formatting.None);
        yield return StartCoroutine(PostRequest($"api/compute/{clusterId}", json, SetResponseCluster));
    }

    void SetResponseCluster(string json)
    {
        responseCluster = new GI_Cluster(json);
        UpdateMeshes();
    }

    public static void Log(string message)
    {
        Debug.Log(message);
    }

    void UpdateMeshes()
    {
        List<Mesh> meshes = new();

        foreach (var output in responseCluster.outputs)
            foreach (var dataItem in output.data)
            {
                if (dataItem is MeshData meshData)
                    meshes.Add(meshData.mesh.UnityMesh);
            }


        foreach (var obj in goo)
            Destroy(obj);

        goo.Clear();
        Resources.UnloadUnusedAssets();

        foreach (var mesh in meshes)
        {
            GameObject go = new();
            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshRenderer>().material = mat;
            go.name = "GrasshopperInside Mesh Output";
            go.transform.SetParent(this.transform, false);

            goo.Add(go);
        }
    }

    void UpdateParameterValues()
    {
        foreach (var booleanParameter in booleanParameters)
            booleanParameter.oldValue = booleanParameter.value;

        foreach (var integerParameter in integerParameters)
            integerParameter.oldValue = integerParameter.value;

        foreach (var numberParameter in numberParameters)
            numberParameter.oldValue = numberParameter.value;
    }

    #region REST API GET & POST Request Functions

    IEnumerator GetRequest(string endPoint, Action<string> callback = null, Action<string> fallback = null)
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{url}:{port}/{endPoint}");

        request.timeout = 5;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            callback?.Invoke(json);
            // Debug.Log(json);
        }

        else
        {
            fallback?.Invoke(request.error);
            // Debug.LogError($"Request failed: {request.error}");
        }
    }

    IEnumerator PostRequest(string endPoint, string payload, Action<string> callback = null, Action<string> fallback = null)
    {
        using UnityWebRequest request = new($"{url}:{port}/{endPoint}", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 5;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            callback?.Invoke(json);
            Debug.Log("POST request successful!");
        }
        else
        {
            fallback?.Invoke(request.error);
            Debug.LogError($"POST request failed: {request.error}");
        }
    }

    #endregion

}

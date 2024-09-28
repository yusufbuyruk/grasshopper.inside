using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO.Enumeration;
using PlasticPipe.PlasticProtocol.Messages;

public class GrasshopperInsideEditor : EditorWindow
{
    private string url = "http://localhost";
    private int port = 4444;

    private readonly HttpClient client = new();

    private string[] documents = new string[] { };
    private int documentId = 0;

    private string[] clusters = new string[] { };
    private int clusterId = 0;

    private GameObject go = null;

    private string message = "";
    public void SetMessage(string msg)
    {
        message = msg;
        Repaint();
    }

    [MenuItem("Grasshopper.Inside/Create Game Object")]
    public static void ShowWindow()
    {
        var window = GetWindow(typeof(GrasshopperInsideEditor));
        window.titleContent = new GUIContent("Grasshopper.Inside");
    }

    private void OnGUI()
    {
        bool isConnected = documents.Length > 0;
        bool isLoaded = clusters.Length > 0;

        GUILayout.Label("Connect to Server", EditorStyles.boldLabel);

        GUI.enabled = !isConnected;
        url = EditorGUILayout.TextField("URL", url);
        port = EditorGUILayout.IntField("Port", port);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(155);

        if (GUILayout.Button("Connect"))
        {
            _ = GetRequest("api/documents", SetDocuments, () => SetMessage("Connection failed"));
        }
        GUI.enabled = isConnected;
        if (GUILayout.Button("Disconnect"))
        {
            ClearDocuments();
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;

        // isConnected = EditorGUILayout.Toggle("Toggle Button", isConnected);

        if (isConnected)
        {
            GUILayout.Space(10);

            GUILayout.Label("Documents", EditorStyles.boldLabel);

            GUI.enabled = !isLoaded;
            documentId = EditorGUILayout.Popup("Document", documentId, documents);


            // Load Document
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(155);

            if (GUILayout.Button("Load Document"))
            {
                _ = GetRequest($"api/load/{documentId}", LoadDocument, ClearClusters);
            }

            GUI.enabled = isLoaded;
            if (GUILayout.Button("Unload"))
            {
                ClearClusters();
            }

            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            if (clusters.Length > 0)
            {
                GUILayout.Space(10);

                GUILayout.Label("Clusters", EditorStyles.boldLabel);

                clusterId = EditorGUILayout.Popup("Document", clusterId, clusters);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(155);

                if (GUILayout.Button("Create Game Object"))
                {
                    _ = GetRequest($"api/getcluster/{clusterId}", CreateGameObject);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Label(message);
    }

    public void SetDocuments(string json)
    {
        DocumentData documentList = JsonConvert.DeserializeObject<DocumentData>(json);
        documents = documentList.documents.ToArray();
        documentId = 0;

        clusters = new string[] { };
        clusterId = -1;
        message = "";
        Repaint();
    }

    public void ClearDocuments()
    {
        documents = new string[] { };
        documentId = -1;

        clusters = new string[] { };
        clusterId = -1;
        Repaint();

        if (go != null)
        {
            DestroyImmediate(go);
        }
    }

    public void LoadDocument(string json)
    {
        ClusterData clusterList = JsonConvert.DeserializeObject<ClusterData>(json);
        clusters = clusterList.labels.ToArray();
        clusterId = 0;
        Repaint();
    }

    public void ClearClusters()
    {
        clusters = new string[] { };
        clusterId = -1;

        if (go != null)
        {
            DestroyImmediate(go);
        }

    }

    public void CreateGameObject(string json)
    {
        DestroyImmediate(go);
        string filename = documents[documentId];
        filename = filename.Replace(".ghx", "");
        filename = filename.Replace(".gh", "");
        go = new($"{filename} - {clusters[clusterId]}");

        var component = go.AddComponent<GI_Component>();
        component.SetConnectionInfo(url, port, documentId, clusterId);
        component.documentName = documents[documentId];
        component.clusterName = clusters[clusterId];

        Material material = Resources.Load<Material>("Materials/GI_MeshMaterial");

        if (material != null)
        {
            component.mat = material;
        }

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            go.transform.SetParent(selectedObject.transform);
        }
        Debug.Log(json);
        Repaint();
    }

    #region REST API HTTP Request Functions

    public async Task GetRequest(string endPoint, Action<string> callback = null, Action fallback = null)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(2));  // Timeout

        try
        {
            HttpResponseMessage response = await client.GetAsync($"{url}:{port}/{endPoint}", cts.Token);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                callback?.Invoke(responseBody);
            }
            else
            {
                fallback?.Invoke();
                Debug.Log(response.StatusCode);
            }
        }

        catch (Exception ex) // HttpRequestException, TaskCanceledException
        {
            fallback?.Invoke();
            Debug.Log(ex.Message);
        }
    }

    public async Task PostRequest(string endPoint, string json, Action<string> callback = null, Action fallback = null)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(2));  // Timeout

        try
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{url}:{port}/{endPoint}", content, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                callback?.Invoke(responseBody);
            }
            else
            {
                fallback?.Invoke();
                Debug.Log(response.StatusCode);
            }
        }
        catch (Exception ex) // HttpRequestException, TaskCanceledException
        {
            fallback?.Invoke();
            Debug.Log(ex.Message);
        }
    }
    #endregion
}



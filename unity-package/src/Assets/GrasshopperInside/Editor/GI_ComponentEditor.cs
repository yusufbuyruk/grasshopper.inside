using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GI_Component))]
public class GI_ComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GI_Component giComponent = (GI_Component)target;

        EditorGUI.BeginDisabledGroup(true);
        giComponent.url = EditorGUILayout.TextField("URL", giComponent.url);
        giComponent.port = EditorGUILayout.IntField("Port", giComponent.port);
        giComponent.documentName = EditorGUILayout.TextField("Document", giComponent.documentName);
        giComponent.clusterName = EditorGUILayout.TextField("Cluster", giComponent.clusterName);
        EditorGUI.EndDisabledGroup();


        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fixedHeight = 32,
            margin = new RectOffset(40, 40, 10, 10) // left, right, top, bottom
        };

        GUIContent buttonContentLoadDocument = new("Load Document", $"Document: {giComponent.documentName} Cluster: {giComponent.clusterName}");

        if (GUILayout.Button(buttonContentLoadDocument, buttonStyle))
        {
            giComponent.LoadDocument();
        }

        GUIContent buttonContentAddCamera = new("Add Simple Camera Controller", "Add simple camera controller component to main camera");

        if (GUILayout.Button(buttonContentAddCamera, buttonStyle))
        {
            GameObject mainCamera = Camera.main.gameObject;

            if (mainCamera.GetComponent<SimpleCameraController>() == null)
            {
                mainCamera.AddComponent<SimpleCameraController>();
            }
        }

        giComponent.mat = (Material)EditorGUILayout.ObjectField("Material", giComponent.mat, typeof(Material), false);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(giComponent);
        }

        // base.OnInspectorGUI();
    }
}
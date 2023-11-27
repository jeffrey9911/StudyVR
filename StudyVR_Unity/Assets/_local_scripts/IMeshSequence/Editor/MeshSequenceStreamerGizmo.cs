using Codice.CM.WorkspaceServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MeshSequenceStreamerGizmo : EditorWindow
{
    [MenuItem("Assets/IAirtable/Create or Rebuild All Asset Bundles")]
    private static void GenerateAssetBundle()
    {
        string BuildPath = Application.dataPath + "/GeneratedAssetBundles";

        try
        {
            BuildPipeline.BuildAssetBundles(BuildPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }




    [MenuItem("Window/IMeshSequence Gizmo")]
    public static void ShowWindow()
    {
        MeshSequenceStreamerGizmo window = GetWindow<MeshSequenceStreamerGizmo>("IMeshSequence Gizmo");
        window.minSize = new Vector2(500, 200);
    }

    string SequencePath = "";
    string ExportPath = "";
    string ExportName = "";
    string ExportFilePath = "";

    private void OnGUI()
    {
        GUILayout.Space(20);

        SequencePath = EditorGUILayout.TextField("Sequence Path", SequencePath);

        if (GUILayout.Button("Load Sequence Folder Path"))
        {
            string path = EditorUtility.OpenFolderPanel("Open a folder that contains a Geometry Sequence (.obj)", SequencePath, "");
            SequencePath = path;
        }

        GUILayout.Space(20);

        ExportPath = EditorGUILayout.TextField("Export Path", ExportPath);

        if (GUILayout.Button("Export Sequence Folder Path"))
        {
            string path = EditorUtility.OpenFolderPanel("Open a folder that the asset bundle will be exported", ExportPath, "");
            ExportPath = path;
        }

        GUILayout.Space(20);

        ExportName = EditorGUILayout.TextField("Export Name", ExportName);

        if (GUILayout.Button("Create MeshSequence Bundle"))
        {
            ExportFilePath = System.IO.Path.Combine(ExportPath, ExportName + ".unity3d");

            string[] ObjPath = Directory.GetFiles(SequencePath, "*.obj");

            if (ObjPath.Length > 0)
            {
                try
                {
                    MeshSequenceStreamer meshSequenceStreamer = new GameObject().AddComponent<MeshSequenceStreamer>();
                    meshSequenceStreamer.EditorLoadMeshSequenceInfo(SequencePath, null, HandleLoadedObject);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }


    void HandleLoadedObject(GameObject gobj, string fileLink)
    {
        PrefabUtility.SaveAsPrefabAsset(gobj, "Assets/Resources/GeneratedPrefabs/" + ExportName + ".prefab");
        CreateAssetBundle(ExportPath, ExportName);
    }

    public static void CreateAssetBundle(string assetBundlePath, string assetBundleName)
    {
        GameObject prefab = Resources.Load<GameObject>($"GeneratedPrefabs/{assetBundleName}");

        if (prefab == null)
        {
            Debug.LogError("Prefab not found. Make sure it's in a Resources folder.");
            return;
        }

        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = assetBundleName;
        buildMap[0].assetNames = new string[] { AssetDatabase.GetAssetPath(prefab) };

        BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;

        BuildPipeline.BuildAssetBundles(assetBundlePath, buildMap, buildOptions, BuildTarget.Android);

        AssetDatabase.Refresh();

        Debug.Log("Asset bundle created at: " + assetBundlePath + assetBundleName);

    }
}

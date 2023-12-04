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
    string SequenceName = "";

    float progressBarPercent = 0;
    string progressBarText = "";

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

        if (GUILayout.Button("Create MeshSequence Bundle"))
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
            LoadObjects(SequencePath, ExportAssetBundle);
        }

        GUILayout.Space(20);

        EditorGUI.ProgressBar(new Rect(3, position.height - 20, position.width - 6, 20), progressBarPercent, progressBarText);
    }

    

    void ExportAssetBundle(GameObject gobj)
    {
        MeshSequenceLoader msl = gobj.GetComponent<MeshSequenceLoader>();
        if(msl != null)
        {
            DestroyImmediate(msl);
        }

        if (!Directory.Exists(Application.dataPath + $"/Resources/IMESHCACHE/GeneratedPrefabs/"))
        {
            Directory.CreateDirectory(Application.dataPath + $"/Resources/IMESHCACHE/GeneratedPrefabs/");
        }


        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gobj, "Assets/Resources/IMESHCACHE/GeneratedPrefabs/" + SequenceName + ".prefab");


        AssetBundleBuild build = new AssetBundleBuild
        {
            assetNames = new string[] { AssetDatabase.GetAssetPath(prefab) },
            assetBundleName = $"iMS_{SequenceName}"
        };

        ExportPath += $"/{SequenceName}";

        if (!Directory.Exists(ExportPath))
        {
            Directory.CreateDirectory(ExportPath);
        }


        BuildPipeline.BuildAssetBundles(ExportPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.Android);

        AssetDatabase.Refresh();

        Debug.Log("Asset bundle created!");

        ClearDirectory(Application.dataPath + $"/Resources/IMESHCACHE/");
        DestroyImmediate(gobj);
    }


    void LoadObjects(string objFolderPath, Action<GameObject> OnObjectsLoaded = null, Action<float> Progress = null)
    {
        if(Directory.Exists(objFolderPath))
        {
            string[] ObjPaths = Directory.GetFiles(objFolderPath, "*.obj");
            string[] MtlPaths = Directory.GetFiles(objFolderPath, "*.mtl");
            string[] TexPaths = Directory.GetFiles(objFolderPath, "*.jpg");

            string[] AudioPaths = Directory.GetFiles(objFolderPath, "*.wav");

            if(ObjPaths.Length > 0)
            {
                SetSequenceName(ObjPaths[0]);

                foreach(string FramePath in ObjPaths)
                {
                    LoadFileToResource(FramePath);
                }

                if(MtlPaths.Length > 0)
                {
                    foreach (string FramePath in MtlPaths)
                    {
                        LoadFileToResource(FramePath);
                    }
                }

                if (TexPaths.Length > 0)
                {
                    foreach (string FramePath in TexPaths)
                    {
                        LoadFileToResource(FramePath);
                    }
                }

                
            }
        }
        else
        {
            Debug.LogError($"Folder doesn't exist! {objFolderPath}");
        }

        UnityEditor.AssetDatabase.Refresh();

        MeshSequenceLoader meshSequenceLoader = new GameObject("MeshSequenceLoader").AddComponent<MeshSequenceLoader>();
        meshSequenceLoader.LoadMeshSequence(SequenceName, OnObjectsLoaded);
    }

    void LoadFileToResource(string filePath)
    {
        if(!Directory.Exists(Application.dataPath + $"/Resources/IMESHCACHE/{SequenceName}/"))
        {
            Directory.CreateDirectory(Application.dataPath + $"/Resources/IMESHCACHE/{SequenceName}/");
        }

        byte[] fileData = File.ReadAllBytes(filePath);

        string destinationPath = Path.Combine(Application.dataPath + $"/Resources/IMESHCACHE/{SequenceName}/", Path.GetFileName(filePath));

        File.WriteAllBytes(destinationPath, fileData);
    }

    void SetSequenceName(string filePath)
    {
        string name = Path.GetFileNameWithoutExtension(filePath);

        int IndexDigits = 0;

        for (int i = name.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(name[i])) IndexDigits++;
            else break;
        }

        SequenceName = name.Substring(0, name.Length - IndexDigits);
    }

    void ClearDirectory(string path)
    {
        try
        {
            // Check if the directory exists
            if (Directory.Exists(path))
            {
                // Delete all files in the directory
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    File.Delete(file);
                }

                // Delete all subdirectories and their contents
                string[] subdirectories = Directory.GetDirectories(path);
                foreach (string directory in subdirectories)
                {
                    ClearDirectory(directory);
                    Directory.Delete(directory);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error clearing persistent data: " + e.Message);
        }
    }

    void ClearAssetBundleNames()
    {
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();

        foreach(string assetPath in assetPaths)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);

            if(assetImporter != null)
            {
                assetImporter.SetAssetBundleNameAndVariant(null, null);
            }
        }

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
    }
}

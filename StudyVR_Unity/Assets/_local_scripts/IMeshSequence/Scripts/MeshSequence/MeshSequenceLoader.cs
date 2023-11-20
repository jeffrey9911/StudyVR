using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Dummiesman;
using UnityEditor;
using UnityEngine;

public class MeshSequenceLoader : MonoBehaviour
{
    [SerializeField] public string MeshSequenceFolder;

    private MeshSequenceContainer meshSequenceContainer;

    public void LoadMeshSequenceByPath(string folderPath = "")
    {
        if(folderPath == "") { folderPath = MeshSequenceFolder; }

        if(Directory.Exists(folderPath))
        {
            string[] objFiles = Directory.GetFiles(folderPath, "*.obj");

            try
            {
                if(objFiles.Length > 0)
                {
                    meshSequenceContainer = this.gameObject.AddComponent<MeshSequenceContainer>();

                    foreach (string objFile in objFiles)
                    {
                        GameObject loadedObject = new OBJLoader().Load(objFile);

                        Mesh frameMesh = GetMesh(loadedObject.transform);
                        if (frameMesh != null) { meshSequenceContainer.MeshSequence.Add(frameMesh); }

                        MeshRenderer frameRenderer = GetMeshRenderer(loadedObject.transform);
                        if (frameRenderer != null)
                        {
                            if(frameRenderer.sharedMaterials.Length > 0)
                            {
                                //meshSequenceContainer.MaterialSequence.Add(frameRenderer.sharedMaterials[0]);
                            }
                        }

                        DestroyImmediate(loadedObject);
                    }

                    this.gameObject.AddComponent<MeshSequencePlayer>();
                    this.gameObject.AddComponent<MeshFilter>();
                    this.gameObject.AddComponent<MeshRenderer>();


                    DestroyImmediate(this.gameObject.GetComponent<MeshSequenceLoader>());
                }
                else
                {
                    Debug.LogError("No .obj files found in folder!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading .obj files: {e}");
            }
        }
        else
        {
            Debug.LogError($"Folder doesn't exist! {folderPath}");
        }
    }


/*
    [ContextMenu("Load Mesh Sequence")]
    public void LoadMeshSequence()
    {
#if UNITY_EDITOR
        if(ExampleMesh != null)
        {
            ExampleMeshName = ExampleMesh.name;
            FolderName = AssetDatabase.GetAssetPath(ExampleMesh).Replace("Assets/Resources/IMeshSequence/", "").Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(ExampleMesh)), "");
            meshSequenceContainer = this.gameObject.AddComponent<MeshSequenceContainer>();
        }
        else
        {
            Debug.LogError("Example Object Mesh is null!");
            return;
        }


        int IndexDigits = 0;

        for(int i = ExampleMeshName.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(ExampleMeshName[i])) IndexDigits++;
            else break;
        }

        string MeshName = ExampleMeshName.Substring(0, ExampleMeshName.Length - IndexDigits);

        FolderBasePath += $"{FolderName}";

        Debug.Log($"Folder Base Path: {FolderBasePath}, Mesh Name Loaded: {MeshName}, Index format: {(0).ToString($"D{IndexDigits}")}");


        while(true)
        {
            if (Resources.Load<GameObject>(FolderBasePath + MeshName + Frames.ToString($"D{IndexDigits}")) == null) break;
            Frames++;
        }

        for (int i = 0; i < Frames; i++)
        {
            GameObject frame = Resources.Load<GameObject>(FolderBasePath + MeshName + i.ToString($"D{IndexDigits}"));
            
            Mesh frameMesh = GetMesh(frame.transform);
            if (frameMesh != null) { meshSequenceContainer.MeshSequence.Add(frameMesh); }

            MeshRenderer frameRenderer = GetMeshRenderer(frame.transform);
            if (frameRenderer != null)
            { 
                if(frameRenderer.sharedMaterials.Length > 0)
                {
                    meshSequenceContainer.MaterialSequence.Add(frameRenderer.sharedMaterials[0]);
                }
            }
        }

        exampleObj = Instantiate(ExampleMesh, this.transform);
        exampleObj.name = "Example Mesh for modify offset";

        Debug.Log("Mesh Sequence Loaded!");

        this.gameObject.AddComponent<MeshSequencePlayer>();

        DestroyImmediate(this.gameObject.GetComponent<MeshSequenceLoader>());
#endif
    }
*/

    private MeshRenderer GetMeshRenderer(Transform parent)
    {
        MeshRenderer renderer = parent.GetComponent<MeshRenderer>();

        if (renderer != null) { return renderer; }

        foreach (Transform child in parent)
        {
            renderer = GetMeshRenderer(child);
            if (renderer != null) { return renderer; }
        }

        return null;
    }

    private Mesh GetMesh(Transform parent)
    {
        MeshFilter meshFilter;

        if(parent.TryGetComponent<MeshFilter>(out meshFilter))
        {
            return meshFilter.sharedMesh;
        }
        else
        {
            foreach (Transform child in parent)
            {
                Mesh mesh = GetMesh(child);
                if (mesh != null) { return mesh; }
            }
        }

        return null;
    }
}

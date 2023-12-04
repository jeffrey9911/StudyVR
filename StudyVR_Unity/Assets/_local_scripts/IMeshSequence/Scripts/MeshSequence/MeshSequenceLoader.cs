using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Dummiesman;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MeshSequenceLoader : MonoBehaviour
{
    private MeshSequenceContainer meshSequenceContainer;


    [ContextMenu("Load Mesh Sequence")]
    public void LoadMeshSequence(string SequenceName, Action<GameObject> OnLoad = null)
    {
#if UNITY_EDITOR
        string[] objPaths = Directory.GetFiles(Application.dataPath + $"/Resources/IMESHCACHE/{SequenceName}/", "*.obj");

        if(objPaths.Length > 0)
        {
            meshSequenceContainer = this.AddComponent<MeshSequenceContainer>();
        }

        foreach(string objPath in objPaths)
        {
            GameObject frame = Resources.Load<GameObject>(objPath.Replace(Application.dataPath + "/Resources/", "").Replace(".obj", ""));

            Mesh frameMesh = GetMesh(frame.transform);
            if (frameMesh != null) { meshSequenceContainer.MeshSequence.Add(frameMesh); }

            MeshRenderer frameRenderer = GetMeshRenderer(frame.transform);
            if (frameRenderer != null)
            {
                if (frameRenderer.sharedMaterials.Length > 0)
                {
                    meshSequenceContainer.MaterialSequence.Add(frameRenderer.sharedMaterials[0]);
                }
            }
        }


        Debug.Log("Mesh Sequence Loaded!");

        this.gameObject.AddComponent<MeshSequencePlayer>();
        this.gameObject.AddComponent<MeshFilter>();
        this.gameObject.AddComponent<MeshRenderer>();

        if(OnLoad == null)
        {
            DestroyImmediate(this.gameObject.GetComponent<MeshSequenceLoader>());
        }
        else
        {
            OnLoad(this.gameObject);
        }

#endif
    }


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

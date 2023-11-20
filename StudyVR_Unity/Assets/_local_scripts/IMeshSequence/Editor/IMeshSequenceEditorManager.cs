using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshSequenceLoader))]
public class IMeshSequenceLoaderEditorManager : Editor
{

    SerializedProperty MeshSequenceFolder;
    MeshSequenceLoader meshSequenceLoader;

    private void OnEnable()
    {
        MeshSequenceFolder = serializedObject.FindProperty("MeshSequenceFolder");

        meshSequenceLoader = (MeshSequenceLoader)target;

        serializedObject.ApplyModifiedProperties();
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Load Sequence Folder Path"))
        {
            string path = EditorUtility.OpenFolderPanel("Open a folder that contains a Geometry Sequence (.obj)", meshSequenceLoader.MeshSequenceFolder, "");
            MeshSequenceFolder.stringValue = path;
        }

        if (GUILayout.Button("Load Sequenced Mesh"))
        {
            meshSequenceLoader.LoadMeshSequenceByPath();
        }

        if(meshSequenceLoader != null)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}




[CustomEditor(typeof(MeshSequenceStreamer))]
public class IMeshSequenceStreamerEditorManager : Editor
{

    SerializedProperty meshSequencePath;
    MeshSequenceStreamer meshSequenceStreamer;

    private void OnEnable()
    {
        meshSequencePath = serializedObject.FindProperty("MeshSequencePath");

        meshSequenceStreamer = (MeshSequenceStreamer)target;

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Load Sequence Folder Path"))
        {
            string path = EditorUtility.OpenFolderPanel("Open a folder that contains a Geometry Sequence (.obj)", meshSequenceStreamer.MeshSequencePath, "");
            meshSequencePath.stringValue = path;
        }

        if (GUILayout.Button("Load Sequenced Mesh"))
        {
            meshSequenceStreamer.StartLoad();
        }

        serializedObject.ApplyModifiedProperties();
    }
}




[CustomEditor(typeof(MeshSequencePlayer))]
public class IMeshSequencePlayerEditorManager : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        MeshSequencePlayer meshSequencePlayer = (MeshSequencePlayer)target;

        if (meshSequencePlayer.isPlayingAudio)
        {
            SetPropertyField("PlayerAudio", meshSequencePlayer.isPlayingAudio);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void SetPropertyField(string name, bool set)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(name), set);
    }
}

public class HierarchyContextMenu : Editor
{
    [MenuItem("GameObject/IMeshSequence Streamer", false, 10)]
    static void InstantiateSequenceStreamer()
    {
        GameObject meshSequenceStreamer = new GameObject("IMeshSequence Streamer");
        meshSequenceStreamer.AddComponent<MeshSequenceStreamer>();
        
        if (meshSequenceStreamer != null)
        {
            PrefabUtility.InstantiatePrefab(meshSequenceStreamer);
        }
    }
}
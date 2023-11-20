using Dummiesman;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class MeshSequenceStreamer : MonoBehaviour
{
    [SerializeField] public string MeshSequencePath;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshSequenceContainer meshSequenceContainer;
    
    private AudioSource PlayerAudioSource;

    private string[] FramePaths;
    private int CurrentFrameIndex = 0;
    private int FrameCount = 0;
    OBJLoader OBJLoader = new OBJLoader();

    public void StartLoad()
    {
        LoadMeshSequenceInfo("", SampleProgressCallback);
    }

    private void SampleProgressCallback(float progress)
    {
        Debug.Log($"Progress: {Math.Round(progress, 2)}%");
    }

    
    public void LoadMeshSequenceInfo(string folderPath = "", Action<float> ProgressCallback = null, Action<GameObject, string> OnLoadedCallback = null, string fileLink = "")
    {
        
        RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[System]: Handling Mesh Sequence...");

        if (folderPath == "") { folderPath = MeshSequencePath; }
        else { MeshSequencePath = folderPath; }

        if (Directory.Exists(folderPath))
        {
            FramePaths = Directory.GetFiles(folderPath, "*.obj");
            
            string[] AudioPath = Directory.GetFiles(folderPath, "*.wav");

            if (FramePaths.Length > 0)
            {
                FrameCount = FramePaths.Length;

                if(AudioPath.Length > 0)
                {
                    PlayerAudioSource = this.gameObject.AddComponent<AudioSource>();

                    
                    StartCoroutine(LoadAudio(AudioPath[0].Replace("\\", "/")));
                }

                meshSequenceContainer = this.gameObject.AddComponent<MeshSequenceContainer>();
                meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
                meshFilter = this.gameObject.AddComponent<MeshFilter>();

                StartCoroutine(LoadFrames(ProgressCallback, OnLoadedCallback, fileLink));
            }
        }
    }

    public IEnumerator LoadAudio(string filePath)
    {
        /* // Get Audio Clip from UnityWebRequest
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);

        yield return www.SendWebRequest();

        Debug.Log(filePath);

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading audio file: " + www.error);
        }
        else
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

            if (audioClip != null)
            {
                Debug.Log("Audio clip loaded successfully.");
                PlayerAudioSource.clip = audioClip;
                PlayerAudioSource.loop = true;

                isPlayingAudio = true;
            }
            else
            {
                Debug.LogError("Failed to load audio clip from path: " + filePath);
            }
        }

        www.Dispose();
        */


        
        string url = "file://" + filePath;

        using (WWW www = new WWW(url))
        {
            yield return www;

            if(www.error == null)
            {
                PlayerAudioSource.clip = www.GetAudioClip();
                Debug.Log("Audio clip loaded successfully.");
                PlayerAudioSource.playOnAwake = false;
                PlayerAudioSource.loop = true;

            }
            else
            {
                Debug.LogError("Error loading audio file: " + www.error);
            }
        }
    }


    public IEnumerator LoadFrames(Action<float> ProgressCallback = null, Action<GameObject, string> OnLoadedCallback = null, string fileLink = "")
    {

        while(meshSequenceContainer.MeshSequence.Count < (CurrentFrameIndex + 1))
        {

            //RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[System]: Loading Frames {(1.0f * CurrentFrameIndex / FrameCount) * 100f}%");

            OBJLoader.LoadMatMesh(FramePaths[CurrentFrameIndex], MeshCallback, MatCallback);

            //if (ProgressCallback != null) ProgressCallback.Invoke((1.0f * CurrentFrameIndex / FrameCount) * 100f);

            CurrentFrameIndex = (CurrentFrameIndex + 1) >= FrameCount ? 0 : CurrentFrameIndex + 1;
            
            yield return null;
        }

        
        this.gameObject.AddComponent<MeshSequenceStreamerPlayer>();
        
        if(OnLoadedCallback != null) OnLoadedCallback.Invoke(this.gameObject, fileLink);
        Debug.Log("Mesh Sequence Loaded");
        
        
        Destroy(this);
    }



    private void SwapFrame(bool isReversing = false)
    {
        meshRenderer.sharedMaterial = meshSequenceContainer.MaterialSequence[CurrentFrameIndex];
        meshFilter.mesh = meshSequenceContainer.MeshSequence[CurrentFrameIndex];

        int NextFrame = 0;
        if (isReversing)
        {
            NextFrame = (CurrentFrameIndex - 1) < 0 ? FrameCount - 1 : CurrentFrameIndex - 1;
        }
        else
        {
            NextFrame = (CurrentFrameIndex + 1) >= FrameCount ? 0 : CurrentFrameIndex + 1;
        }

        CurrentFrameIndex = NextFrame;
    }

    void MatCallback(Material[] mats)
    {
        meshSequenceContainer.MaterialSequence.Add(mats[0]);
    }

    void MeshCallback(Mesh msf)
    {
        meshSequenceContainer.MeshSequence.Add(msf);
    }
}

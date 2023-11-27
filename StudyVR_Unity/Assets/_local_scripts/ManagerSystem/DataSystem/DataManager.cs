using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class PreloadObject
{
    public string ObjectLink;
    public GameObject ObjectInstance;

    public PreloadObject(string olink, GameObject gobj)
    {
        ObjectLink = olink;
        ObjectInstance = gobj;
    }
}

public enum StudyType
{
    Noset,
    Evualuation,
    Comparison
}

public class DataManager : MonoBehaviour
{
    public void OnConfigLoaded()
    {
        RuntimeManager.Instance.UI_MANAGER.ConfigLayer.LoadConfigsToDD();
    }

    private string PendingLink1 = "";
    private string PendingLink2 = "";
    private List<PreloadObject> PreloadObjects = new List<PreloadObject>();
    private int CurrentPreloadIndex = 0;
    private int CurrentPreloadCount = 0;
    public bool IsPreloaded = false;

    public record_data LoadedRecord;
    public StudyType CurrentStudyType = StudyType.Noset;

    public void LoadRecordData(record_data rd)
    {
        LoadedRecord = rd;

        StartPreload();
    }

    public GameObject GetPreloadedObject(string olink)
    {
        foreach(PreloadObject preloadObject in PreloadObjects)
        {
            if(preloadObject.ObjectLink == olink)
            {
                return preloadObject.ObjectInstance;
            }
        }

        return null;
    }

    public void StartPreload()
    {
        PendingLink1 = StudyVR_IAirtable.studyvr_config_records.records[RuntimeManager.Instance.UI_MANAGER.ConfigLayer.GetRecordIndex].fields.AssetLink1;
        PendingLink2 = StudyVR_IAirtable.studyvr_config_records.records[RuntimeManager.Instance.UI_MANAGER.ConfigLayer.GetRecordIndex].fields.AssetLink2;

        bool isAsset1Loaded = false;
        bool isAsset2Loaded = false;

        foreach(PreloadObject preloadObject in PreloadObjects)
        {
            if(preloadObject.ObjectLink == PendingLink1)
            {
                isAsset1Loaded = true;
            }

            if (preloadObject.ObjectLink == PendingLink2)
            {
                isAsset2Loaded = true;
            }
        }

        if(PendingLink2 == null)
        {
            isAsset2Loaded = true;
        }

        if(isAsset1Loaded && isAsset2Loaded)
        {
            FinishPreload();
            return;
        }
        else if(!isAsset2Loaded)
        {
            CurrentStudyType = StudyType.Comparison;
            CurrentPreloadCount = 2;
        }
        else
        {
            CurrentStudyType = StudyType.Evualuation;
            CurrentPreloadCount = 1;
        }


        Preload();
    }

    private void Preload()
    {
        if(CurrentPreloadIndex >= CurrentPreloadCount)
        {
            FinishPreload();
            return;
        }

        switch(CurrentPreloadIndex)
        {
            case 0:
                //StartCoroutine(GetAssetBundleFromGoogleDrive(PendingLink1, PreloadCallback, PreloadProgressCallback));
                StartCoroutine(GetDataFromGoogleDrive(PendingLink1, PreloadProgressCallback));
                break;

            case 1:
                //StartCoroutine(GetAssetBundleFromGoogleDrive(PendingLink2, PreloadCallback, PreloadProgressCallback));
                StartCoroutine(GetDataFromGoogleDrive(PendingLink1, PreloadProgressCallback));
                break;

            default:
                break;
        }
    }

    private void PreloadCallback(GameObject gobj, string link)
    {
        PreloadObjects.Add(new PreloadObject(link, gobj));
        CurrentPreloadIndex++;

        Preload();
    }

    private void PreloadProgressCallback(float progress)
    {
        RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[System]: Preloading objects [{CurrentPreloadIndex + 1}/{CurrentPreloadCount}] - {(int)(progress * 100)}%");
    }

    private void FinishPreload()
    {
        RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[System]: Preload finished.");
        IsPreloaded = true;
        RuntimeManager.Instance.UI_MANAGER.StartUserCanvas(RuntimeManager.Instance.UI_MANAGER.ConfigLayer.IsTutorial);
    }

    private IEnumerator GetAssetBundleFromGoogleDrive(string fileLink, Action<GameObject, string> callback, Action<float> progressCallback = null)
    {
        string fileID = ExtractFileIdFromGoogleDriveLink(fileLink);
        if(string.IsNullOrEmpty(fileID))
        {
            RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage("[ERROR]: Preload failed. Wrong link.");
            yield break;
        }

        GameObject gameObject = null;

        string url = $"https://www.googleapis.com/drive/v3/files/{fileID}?alt=media&key={RuntimeManager.Instance.STUDYVR_IAIRTABLE.GoogleAPIKey}";

        UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);

        AsyncOperation asyncOperation = webRequest.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            progressCallback?.Invoke(Mathf.Clamp01(asyncOperation.progress));
            yield return null;
        }

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);

            if (bundle != null)
            {
                string[] assetNames = bundle.GetAllAssetNames();

                if (assetNames.Length > 0)
                {
                    gameObject = bundle.LoadAsset<GameObject>(assetNames[0]);

                    callback?.Invoke(gameObject, fileLink);
                }
            }
        }
    }

    
    private IEnumerator GetDataFromGoogleDrive(string fileLink, Action<float> progressCallback = null)
    {
        string fileID = ExtractFileIdFromGoogleDriveLink(fileLink);
        if(string.IsNullOrEmpty(fileID))
        {
            RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage("[ERROR]: Preload failed. Wrong link.");
            yield break;
        }
        

        string url = $"https://www.googleapis.com/drive/v3/files/{fileID}?alt=media&key={RuntimeManager.Instance.STUDYVR_IAIRTABLE.GoogleAPIKey}";

        UnityWebRequest webRequest = UnityWebRequest.Get(url);

        AsyncOperation asyncOperation = webRequest.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            progressCallback?.Invoke(Mathf.Clamp01(asyncOperation.progress));
            yield return null;
        }

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            byte[] data = webRequest.downloadHandler.data;

            switch(GetFileType(data))
            {
                case "zip":
                    Debug.Log("Extracting Zip File");
                    string extreactPath = HandleZip(fileID, data);

                    HandleExtractedFile(fileLink, extreactPath);
                    break;

                case "Defult":
                    HandleAssetBundle(fileID, fileLink, data);
                    break;

                default:
                    break;
            }
        }
    }


    string ExtractFileIdFromGoogleDriveLink(string link)
    {
        string pattern = @"/file/d/([^/]+)/";

        Match match = Regex.Match(link, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty;
    }

    string GetFileType(byte[] data)
    {
        byte[] zipMB = new byte[] { 0x50, 0x4B, 0x03, 0x04 };

        if(StartsWithBytes(data, zipMB))
        {
            return "zip";
        }
        else
        {
            return "Defult";
        }


    }

    bool StartsWithBytes(byte[] data, byte[] pattern)
    {
        if(data.Length < pattern.Length)
        {
            return false;
        }

        for(int i = 0; i < pattern.Length; i++)
        {
            if(data[i] != pattern[i])
            {
                return false;
            }
        }

        return true;
    }

    void HandleAssetBundle(string fileID, string fileLink, byte[] data)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, $"{fileID}.assetbundle");

        System.IO.File.WriteAllBytes(path, data);

        AssetBundle bundle = AssetBundle.LoadFromFile(path);

        if (bundle != null)
        {
            string[] assetNames = bundle.GetAllAssetNames();
            GameObject gameObject;

            if (assetNames.Length > 0)
            {
                gameObject = bundle.LoadAsset<GameObject>(assetNames[0]);

                HandleLoadedObject(gameObject, fileLink);
            }
        }
    }

    private string HandleZip(string fileID, byte[] data)
    {
        RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[System]: Handling Zip file...");
        try
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, $"{fileID}.zip");

            System.IO.File.WriteAllBytes(path, data);

            string extractPath = System.IO.Path.Combine(Application.persistentDataPath, fileID);

            ZipFile.ExtractToDirectory(path, extractPath);

            return extractPath;
        }
        catch(Exception e)
        {
            RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[Error]: {e.Message}");
            return null;
        }
    }

    void HandleExtractedFile(string filelink, string extractPath)
    {
        
        string[] ObjPath = Directory.GetFiles(extractPath, "*.obj");
        string[] PlyPath = Directory.GetFiles(extractPath, "*.ply");

        RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"{ObjPath.Length}, {extractPath}");   


        if(ObjPath.Length > 0)
        {
            try
            {
                MeshSequenceStreamer meshSequenceStreamer = new GameObject().AddComponent<MeshSequenceStreamer>();
                meshSequenceStreamer.LoadMeshSequenceInfo(extractPath, MeshSequenceLoadProgress, HandleLoadedObject, filelink);
            }
            catch(Exception e)
            {
                RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[Error]: {e.Message}");
            }
        }

        if(PlyPath.Length > 0)
        {
            try
            {
                MeshSequenceStreamer meshSequenceStreamer = new GameObject().AddComponent<MeshSequenceStreamer>();
                meshSequenceStreamer.LoadMeshSequenceInfo(extractPath, MeshSequenceLoadProgress, HandleLoadedObject, filelink);
            }
            catch(Exception e)
            {
                RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[Error]: {e.Message}");
            }
        }
    }

    private void HandleLoadedObject(GameObject gobj, string fileLink)
    {
        PreloadObjects.Add(new PreloadObject(fileLink, gobj));
        PreloadObjects[PreloadObjects.Count - 1].ObjectInstance.SetActive(false);
        CurrentPreloadIndex++;
        
        Preload();
    }

    void MeshSequenceLoadProgress(float progress)
    {
        RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[System]: Loading mesh sequence {Math.Round(progress, 2)}%");
    }

    void OnApplicationQuit()
    {
        ClearDirectory(Application.persistentDataPath);
    }

    public void OnClearCacheClicked()
    {
        try
        {
            ClearDirectory(Application.persistentDataPath);
        }
        catch(Exception e)
        {
            RuntimeManager.Instance.UI_MANAGER.ConfigLayer.UISystemMessage($"[Error]: {e.Message}");
        }
    }

    private void ClearDirectory(string path)
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            if (preloadObject.ObjectLink == PendingLink2 || PendingLink2 == null)
            {
                isAsset2Loaded = true;
            }
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
                StartCoroutine(GetAssetBundleFromGoogleDrive(PendingLink1, PreloadCallback, PreloadProgressCallback));
                break;

            case 1:
                StartCoroutine(GetAssetBundleFromGoogleDrive(PendingLink2, PreloadCallback, PreloadProgressCallback));
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
}

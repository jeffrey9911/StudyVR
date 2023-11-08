using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeManager : MonoBehaviour
{
    public static RuntimeManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    public StudyVR_IAirtable STUDYVR_IAIRTABLE;

    public DataManager DATA_MANAGER;

    public UIManager UI_MANAGER;

    public WebViewManager WEBVIEW_MANAGER;

}

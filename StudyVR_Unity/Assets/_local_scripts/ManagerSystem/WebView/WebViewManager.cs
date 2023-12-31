using System.Collections;
using System.Collections.Generic;
using TLab.Android.WebView;
using UnityEngine;

public class WebViewManager : MonoBehaviour
{
    [SerializeField] private TLabWebView m_webView;

    private bool WebViewEnable = false;

    public void LoadPreStudyWebview()
    {
        m_webView.SetUrl(RuntimeManager.Instance.DATA_MANAGER.LoadedRecord.fields.PreStudyLink);
        m_webView.StartWebView();
        WebViewEnable = true;
    }

    [ContextMenu("LoadMainStudyWebview")]
    public void LoadMainStudyWebview()
    {
        m_webView.SetUrl(RuntimeManager.Instance.DATA_MANAGER.LoadedRecord.fields.MainStudyLink);
        m_webView.LoadUrl(RuntimeManager.Instance.DATA_MANAGER.LoadedRecord.fields.MainStudyLink);
        WebViewEnable = true;

        //StartCoroutine("LoadObject");

        LoadObject();
    }

    void LoadObject()
    {
        RuntimeManager.Instance.OBJECT_MANAGER.SpawnObject();
    }

    void Update()
    {
#if UNITY_ANDROID
        if(WebViewEnable)
        {
            m_webView.UpdateFrame();
        }
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class StudyVR_IAirtable : MonoBehaviour
{
    public static STUDYVR_RECORDS studyvr_config_records;

    [SerializeField] private string AIRTABLE_API_KEY;

    [SerializeField] private string GOOGLE_API_KEY;

    public bool InitializeOnStart = true;

    [HideInInspector] public bool isReady = false;

    public record_data GetRecord(string recordID)
    {
        foreach (record_data record in studyvr_config_records.records)
        {
            if(record.fields.QuestionnaireID == recordID)
            {
                return record;
            }
        }
        return null;
    }

    private IEnumerator InitializeBase()
    {
        string apiurl = "https://api.airtable.com/v0/meta/bases";

        UnityWebRequest webRequest = UnityWebRequest.Get(apiurl);

        webRequest.SetRequestHeader("Authorization", $"Bearer {AIRTABLE_API_KEY}");

        yield return webRequest.SendWebRequest();

        if(webRequest.result == UnityWebRequest.Result.Success)
        {
            string response = webRequest.downloadHandler.text;
            STUDYVR_BASES studyvrBases = JsonUtility.FromJson<STUDYVR_BASES>(response);
            StartCoroutine(InitializeTable(studyvrBases.bases[0].id));
        }
    }

    private IEnumerator InitializeTable(string baseID)
    {
        string apiurl = $"https://api.airtable.com/v0/meta/bases/{baseID}/tables";

        UnityWebRequest webRequest = UnityWebRequest.Get(apiurl);

        webRequest.SetRequestHeader("Authorization", $"Bearer {AIRTABLE_API_KEY}");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            STUDYVR_TABLES studyvrTables = JsonUtility.FromJson<STUDYVR_TABLES>(webRequest.downloadHandler.text);
            StartCoroutine(InitializeRecords(baseID, studyvrTables.tables[0].id));
        }
    }

    private IEnumerator InitializeRecords(string baseID, string tableID)
    {
        string apiurl = $"https://api.airtable.com/v0/{baseID}/{tableID}";

        UnityWebRequest webRequest = UnityWebRequest.Get(apiurl);

        webRequest.SetRequestHeader("Authorization", $"Bearer {AIRTABLE_API_KEY}");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            studyvr_config_records = JsonUtility.FromJson<STUDYVR_RECORDS>(webRequest.downloadHandler.text);

            isReady = true;

            RuntimeManager.Instance.DATA_MANAGER.OnConfigLoaded();

            foreach (record_data record in studyvr_config_records.records)
            {
                Debug.Log("===========");
                Debug.Log("record id: " + record.id);
                Debug.Log("created time: " + record.createdTime);
                Debug.Log("question id: " + record.fields.QuestionnaireID);
                Debug.Log("scene link: " + record.fields.QuestionnaireScene);
                Debug.Log("question link: " + record.fields.PreStudyLink);
                Debug.Log("question link: " + record.fields.MainStudyLink);
                Debug.Log("question as1: " + record.fields.AssetLink1);
                Debug.Log("question as2: " + record.fields.AssetLink2);
                Debug.Log("question comm: " + record.fields.Comments);
            }
        }
    }

    [ContextMenu("Initialize StudyVR Config")]
    public void InitializeStudyVRConfig()
    {
        StartCoroutine(InitializeBase());
    }

    private void Start()
    {
        if(InitializeOnStart)
        {
            StartCoroutine(InitializeBase());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text DebugText;
    [SerializeField] private TMP_InputField ConfigComment;
    [SerializeField] private TMP_Dropdown ConfigDropdown;

    [SerializeField] private TMP_InputField QuestionnaireID;

    [SerializeField] private Toggle TutorialToggle;

    private int CurrentConfigIndex = 0;

    private void Start()
    {
        //ConfigDropdown.onValueChanged.AddListener(OnUnityConfigDDValue);
        TutorialToggle.onValueChanged.AddListener(OnTutorialToggleValue);
    }

    public void LoadConfigsToDD()
    {
        /*
        if(RuntimeManager.Instance.STUDYVR_IAIRTABLE.isReady)
        {
            //ConfigDropdown.options.Clear();
            foreach (record_data record in StudyVR_IAirtable.studyvr_config_records.records)
            {
                ConfigDropdown.options.Add(new TMP_Dropdown.OptionData(record.fields.QuestionnaireID));
            }

        }
        */
        LoadQuestionnaireInfo();
    }

    private void OnTutorialToggleValue(bool value)
    {
        string yes = value.ToString();
    }

    private void OnUnityConfigDDValue(int index)
    {
        record_data SelectedRecord = RuntimeManager.Instance.STUDYVR_IAIRTABLE.GetRecord(ConfigDropdown.options[index].text);
        if(SelectedRecord != null)
        {
            ConfigComment.text = SelectedRecord.fields.Comments;
        }
    }

    private void LoadQuestionnaireInfo()
    {
        record_data SelectedRecord = StudyVR_IAirtable.studyvr_config_records.records[CurrentConfigIndex];

        QuestionnaireID.text = SelectedRecord.fields.QuestionnaireID;
        ConfigComment.text = SelectedRecord.fields.Comments;
    }

    

    public void UISystemMessage(string message)
    {
        DebugText.text = message;
    }

    public void StartOnClick()
    {
        RuntimeManager.Instance.UI_MANAGER.StartUserCanvas(TutorialToggle.isOn);
    }

    public void PreviousConfigOnClick()
    {
        if (CurrentConfigIndex > 0)
        {
            CurrentConfigIndex--;
        }
        else
        {
            CurrentConfigIndex = StudyVR_IAirtable.studyvr_config_records.records.Count - 1;
        }

        LoadQuestionnaireInfo();
    }

    public void NextConfigOnClick()
    {
        if (CurrentConfigIndex < StudyVR_IAirtable.studyvr_config_records.records.Count - 1)
        {
            CurrentConfigIndex++;
        }
        else
        {
            CurrentConfigIndex = 0;
        }

        LoadQuestionnaireInfo();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public ConfigCanvas ConfigLayer;
    [SerializeField] public TutorialManager TutorialLayer;
    [SerializeField] public UserCanvas UserLayer;



    // Start with Config Canvas
    private void Start()
    {
        ConfigLayer.gameObject.SetActive(true);
        TutorialLayer.gameObject.SetActive(false);
        UserLayer.gameObject.SetActive(false);
    }

    // Tutorial Canvas
    [ContextMenu("StartTutorialCanvas")]
    public void StartTutorialCanvas()
    {
        ConfigLayer.gameObject.SetActive(false);
        UserLayer.gameObject.SetActive(false);
        TutorialLayer.gameObject.SetActive(true);

        TutorialLayer.StartTutorial();
    }

    // User Canvas
    public void StartUserCanvas(bool isTutorial)
    {
        if(isTutorial)
        {
            StartTutorialCanvas();
            return;
        }

        ConfigLayer.gameObject.SetActive(false);
        UserLayer.gameObject.SetActive(true);
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public ConfigCanvas ConfigCanvas;
    [SerializeField] public GameObject TutorialCanvas;
    [SerializeField] public UserCanvas UserCanvas;



    // Start with Config Canvas
    private void Start()
    {
        ConfigCanvas.gameObject.SetActive(true);
        //TutorialCanvas.SetActive(false);
        UserCanvas.gameObject.SetActive(false);
    }

    // Tutorial Canvas


    // User Canvas
    public void StartUserCanvas(bool isTutorial)
    {
        ConfigCanvas.gameObject.SetActive(false);
        //TutorialCanvas.SetActive(false);
        UserCanvas.gameObject.SetActive(true);
    }

}

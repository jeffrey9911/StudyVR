using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public void OnConfigLoaded()
    {
        RuntimeManager.Instance.UI_MANAGER.ConfigCanvas.LoadConfigsToDD();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUImanager : MonoBehaviour
{
    public static GlobalUImanager instance;
    public GameObject loadingPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public void ShowLoadingPanel()
    {
        loadingPanel.SetActive(true);
    }

    public void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;

    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public float displayDuration = 2f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void ShowTooltip(string message)
    {
        if (tooltipPanel == null || tooltipText == null)
        {
            Debug.LogError("Tooltip UI is not properly assigned.");
            return;
        }

        tooltipText.text = message;
        tooltipPanel.SetActive(true);
        Invoke(nameof(HideTooltip), displayDuration);
    }

    private void HideTooltip()
    {
        tooltipText.text = "";
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}

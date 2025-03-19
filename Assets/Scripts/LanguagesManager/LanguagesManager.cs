using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguagesManager : MonoBehaviour
{
    public static LanguagesManager instance;
    [SerializeField] private TMP_FontAsset icielFonts;
    [SerializeField] private TMP_FontAsset notoFonts;
    public GameObject LanguagePanel;
    public GameObject languageCanvas;
    void Awake()
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
        int ID = PlayerPrefs.GetInt("Language", 0);
        if (ID == 0)
        {
            SetDefaultLanguage();
        }
        else
        {
            ChangeLanguage(ID);
        }
    }

    public void SetDefaultLanguage()
    {
        var systemLanguage = Application.systemLanguage;
        Debug.Log("System Language: " + systemLanguage.ToString());
        if (systemLanguage == SystemLanguage.English)
        {
            ChangeLanguage(0);
        }
        else if (systemLanguage == SystemLanguage.Vietnamese)
        {
            ChangeLanguage(1);
        }
        else if (systemLanguage == SystemLanguage.ChineseSimplified)
        {
            ChangeLanguage(2);
        }
        else
        {
            ChangeLanguage(0);
        }
    }

    private bool active = false;
    public void ChangeLanguage(int localeID)
    {
        if (active == true)
        {
            return;
        }
        StartCoroutine(SetLanguage(localeID));
    }

    IEnumerator SetLanguage(int localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt("Language", localeID);
        ApplyFontForLanguage(localeID);
        active = false;
    }

    void ApplyFontForLanguage(int localeID)
    {
        TMP_FontAsset selectedFont = GetTMPFontByLocaleID(localeID);

        var textMeshProTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var tmpText in textMeshProTexts)
        {
            tmpText.font = selectedFont;
        }
    }
    
    TMP_FontAsset GetTMPFontByLocaleID(int localeID)
    {
        switch (localeID)
        {
            case 0:
            case 1:
                return icielFonts;
            case 2:
                return notoFonts;
            default:
                return icielFonts;
        }
    }

    public string GetLocalizedString(string key)
    {
        var handle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("StringTable", key);
        return handle.IsDone ? handle.Result : key; // Nếu chưa có chuỗi, trả về key làm dự phòng
    }

    public string GetLocalizedNotification(string key)
    {
        var handle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("NotificationTable", key);
        return handle.IsDone ? handle.Result : key; // Nếu chưa có chuỗi, trả về key làm dự phòng
    }

    public void Language()
    {
        SoundController.instance.PlaySelectSound();
        LanguagePanel.SetActive(true);
    }

    public void CloseLanguage()
    {
        SoundController.instance.PlaySelectSound();
        LanguagePanel.SetActive(false);
    }
}

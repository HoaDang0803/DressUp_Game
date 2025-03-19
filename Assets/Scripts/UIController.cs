using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject dressUpMenu;
    public GameObject model;

    void Start()
    {
        model.SetActive(false);
        mainMenu.SetActive(true);
        dressUpMenu.SetActive(false);
        pnlLoading.SetActive(false);
    }

    public void DressUp()
    {
        SoundController.instance.PlaySelectSound();
        Debug.Log("DressUp");
        Interstitial.instance.ShowInterstitialAd(() =>
       {
           mainMenu.SetActive(false);
           LoadDressUp();
       });
    }

    public void BackToMainMenu()
    {
        SoundController.instance.PlaySelectSound();
        model.SetActive(false);
        mainMenu.SetActive(true);
        dressUpMenu.SetActive(false);
    }

    public void SaveDressUp()
    {
        SoundController.instance.PlaySaveSound();
        Interstitial.instance.ShowInterstitialAd(() =>
       {
           List<SpriteRenderer> modelParts = GameController.instance.modelParts;
           if (modelParts[6].sprite == null && modelParts[7].sprite == null && modelParts[8].sprite == null && modelParts[9].sprite == null && modelParts[10].sprite == null)
           {
               return;
           }
           StartCoroutine(TakeScreenshotAndSwitchMenus());
       });
    }

    private IEnumerator TakeScreenshotAndSwitchMenus()
    {
        Screenshot.instance.TakeScreenshot();
        yield return new WaitForEndOfFrame();

        model.SetActive(false);
        mainMenu.SetActive(true);
        dressUpMenu.SetActive(false);

        GameController.instance.LoadItemsToMainMenu();
    }

    public void ResetItems()
    {
        GameController.instance.ResetItems();
    }

    public GameObject loadingBar;

    public Image loadingBarFill;
    public GameObject pnlLoading;
    private float fakeProgress = 0f;

    public void LoadDressUp()
    {
        pnlLoading.SetActive(true);
        dressUpMenu.SetActive(false);
        StartCoroutine(LoadDressUpAsync());
    }

    IEnumerator LoadDressUpAsync()
    {
        fakeProgress = 0f;
        loadingBar.SetActive(true);

        while (fakeProgress < 1f)
        {
            fakeProgress += Time.deltaTime * 0.5f;
            loadingBarFill.fillAmount = fakeProgress;
            yield return null;
        }

        loadingBar.SetActive(false);
        pnlLoading.SetActive(false);
        dressUpMenu.SetActive(true);
        model.SetActive(true);
    }

    public void GalleryActive()
    {
        Screenshot.instance.OpenGallery();
    }

    public void GalleryInactive()
    {
        SoundController.instance.PlaySelectSound();
        Screenshot.instance.galleryPanel.SetActive(false);
    }

    public void PK()
    {
        SoundController.instance.PlaySelectSound();
        LanguagesManager.instance.languageCanvas.SetActive(false);
        Interstitial.instance.ShowInterstitialAd(() =>
        {
            SceneManager.LoadScene("PK");
        });
    }
}

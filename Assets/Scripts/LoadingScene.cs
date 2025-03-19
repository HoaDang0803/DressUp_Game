using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public GameObject loadingBar;
    public Button start;
    public Image loadingBarFill;
    private AsyncOperation operation;
    private float fakeProgress = 0f;

    void Start()
    {
        StartCoroutine(LoadSceneAsync(0));
    }

    public void LoadScene(int sceneId)
    {
        SoundController.instance.PlaySelectSound();
        Interstitial.instance.ShowInterstitialAd(() =>
        {
            operation.allowSceneActivation = true;
            SceneManager.LoadScene(sceneId);
        });
    }


    IEnumerator LoadSceneAsync(int sceneId)
    {
        operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);

            if (fakeProgress < realProgress)
            {
                fakeProgress += Time.deltaTime * 0.3f;
                fakeProgress = Mathf.Min(fakeProgress, realProgress);
            }
            loadingBarFill.fillAmount = fakeProgress;

            if (fakeProgress >= 1f)
            {
                loadingBar.SetActive(false);
                start.gameObject.SetActive(true);
                yield break;
            }

            yield return null;
        }
    }
}

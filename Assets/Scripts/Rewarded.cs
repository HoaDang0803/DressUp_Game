using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Rewarded : MonoBehaviour
{
    public static Rewarded instance;
    public Button shuffleButton; // Nút Shuffle
    public GameObject adIcon;    // Icon quảng cáo hiện lên khi vượt qua giới hạn free shuffle

    private int freeShuffleCount = 2; // Số lần shuffle miễn phí
    private int currentShuffleCount = 0; // Số lần shuffle đã sử dụng
    private RewardedAd rewardedAd;

#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
  private string _adUnitId = "unused";
#endif
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
    }
    void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        LoadRewardedAd();
        shuffleButton.onClick.AddListener(OnShuffleButtonClicked);

        adIcon.SetActive(false);
    }

    private void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load rewarded ad: " + error);
                return;
            }

            rewardedAd = ad;
            RegisterEventHandlers(ad);
        });
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd();
        };
    }

    private void OnShuffleButtonClicked()
    {
        if (currentShuffleCount < freeShuffleCount)
        {
            Shuffle();
            GameController.instance.RandomizeItems();
        }
        else
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    Shuffle();
                    GameController.instance.RandomizeItems();
                });
            }
            else
            {
                Debug.Log("Ad not ready.");
                SoundController.instance.PlayErrorSound();
                Tooltip.instance.ShowTooltip(LanguagesManager.instance.GetLocalizedString("ads_key"));
                LoadRewardedAd();
            }
        }
    }
    private bool isProcessing = false;
    private async void Shuffle()
    {
        if (isProcessing)
        {
            return;
        }

        isProcessing = true;
        currentShuffleCount++;
        Debug.Log("Shuffling items...");

        if (currentShuffleCount >= freeShuffleCount)
        {
            adIcon.SetActive(true);
        }
        await Task.Delay(1500);
        isProcessing = false;
    }

    public void ShowRewardedAd(Action onAdCompleted = null)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("Ad completed.");
                onAdCompleted?.Invoke();
            });
        }
        else
        {
            Debug.Log("Ad not ready.");
            Tooltip.instance.ShowTooltip(LanguagesManager.instance.GetLocalizedString("ads_key"));
            LoadRewardedAd();
        }
    }

}

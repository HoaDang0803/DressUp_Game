using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class Interstitial : MonoBehaviour
{
    public static Interstitial instance;
    private InterstitialAd _interstitialAd;
    private float _lastAdShownTime;
    private int retryAttempt = 0;
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/4411468910"; 
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

        MobileAds.RaiseAdEventsOnUnityMainThread = true;

    }

    void Start()
    {
        _lastAdShownTime = -FirebaseManager.instance.time_between_ad_full - 10f;
        MobileAds.Initialize(initStatus =>
        {
            LoadInterstitialAd();
        });
    }

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"Quảng cáo không tải được: {error.GetMessage()}");
                    if (retryAttempt < 3) // Giới hạn 3 lần thử
                    {
                        retryAttempt++;
                        Invoke(nameof(LoadInterstitialAd), 0.5f); // Thử lại sau 2 giây
                    }
                    return;
                }

                retryAttempt = 0; // Reset số lần thử khi thành công
                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd);
            });
    }

    public void ShowInterstitialAd(Action onAdCompleted = null)
    {
        float adTime = FirebaseManager.instance.time_between_ad_full;

        if (_interstitialAd != null && _interstitialAd.CanShowAd() && Time.time - _lastAdShownTime >= adTime)
        {
            _lastAdShownTime = Time.time;
            GlobalUImanager.instance.ShowLoadingPanel();
            _interstitialAd.Show();
            _interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                GlobalUImanager.instance.HideLoadingPanel();
                Debug.Log("Quảng cáo đã đóng, thực hiện hành động tiếp theo.");
                onAdCompleted?.Invoke();
            };
        }
        else
        {
            Debug.Log("Quảng cáo chưa sẵn sàng hoặc chưa đủ thời gian: " + adTime + " giây.");
            LoadInterstitialAd();
            onAdCompleted?.Invoke();
        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd();
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Quảng cáo không thể mở với lỗi: " + error + ". Đang tải lại quảng cáo.");
            LoadInterstitialAd();
        };
    }

    private void OnDestroy()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
        }
    }
}



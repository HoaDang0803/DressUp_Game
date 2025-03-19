using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    public float time_between_ad_full = 30f;

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
        CheckRemoteConfigValue();
    }

    public Task CheckRemoteConfigValue()
    {
        Debug.Log("Fetching data...");
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    public void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Fetch failed!");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)}: Failed to fetch remote config. LastFetchStatus: {info.LastFetchStatus}");
            return;
        }

        remoteConfig.ActivateAsync()
        .ContinueWithOnMainThread(task =>
        {
            Debug.Log("Remote config activated");
            time_between_ad_full = remoteConfig.GetValue("time_between_ad_full").LongValue;
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum isAds
{
    True,
    False
}

[System.Serializable]
public class Item
{
    public string id;
    public int elegance;
    public int cute;
    public int cool;
    public int polite;
    public int sexy;
    public bool ads;
}

[System.Serializable]
public class Type
{
    public int id;
    public string text;
    public string icon;
    public List<Item> items;

    public int Length { get; internal set; }
}

[System.Serializable]
public class DressupData
{
    public List<Type> dressup_db;
}

public class DataController : MonoBehaviour
{
    public static DataController instance;

    public DressupData dressupData = new DressupData();

    void Start()
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
        LoadData();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    public void LoadData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Data/Vlinder_Data");

        if (jsonData != null)
        {
            dressupData = JsonUtility.FromJson<DressupData>(jsonData.text);
        }
    }

}


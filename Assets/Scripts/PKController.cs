using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PKController : MonoBehaviour
{
    public static PKController instance;
    public List<PKIcon> icons;
    public List<PKItem> items;
    public List<SpriteRenderer> playerModelParts;
    private PKIcon selectedIcon;
    public Dictionary<int, string> selectedItemsByIcon = new Dictionary<int, string>();
    public List<SpriteRenderer> enemyModelParts;
    public GameObject dressup;
    public GameObject player;
    public GameObject enemy;
    public GameObject loading;
    public int winstreak = 0;
    public ParticleSystem particleEffect;

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
        particleEffect.Stop();
        Time.timeScale = 1f;
        LoadWinStreak();
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitUntil(() => DataController.instance != null && DataController.instance.dressupData != null);
        yield return new WaitUntil(() => PKItemsController.instance != null);
        LoadIcon();
        SelectOnlyThisIcon(icons[0]);
        selectedItemsByIcon[0] = "4011";
        selectedItemsByIcon[1] = "40262";
        selectedItemsByIcon[3] = "40432";
        selectedItemsByIcon[4] = "40526";
        selectedItemsByIcon[5] = "2421";
        selectedItemsByIcon[6] = "4071";
        selectedItemsByIcon[7] = "40822";
        selectedItemsByIcon[9] = "41094";
        playerModelParts[1].sprite = Resources.Load<Sprite>("Items/2_frontHair/40262");
        playerModelParts[3].sprite = Resources.Load<Sprite>("Items/4_eyes/40432");
        playerModelParts[4].sprite = Resources.Load<Sprite>("Items/5_eyebrown/40526");
        playerModelParts[5].sprite = Resources.Load<Sprite>("Items/6_mouth/2421");
        playerModelParts[6].sprite = Resources.Load<Sprite>("Items/7_vest/4071");
        playerModelParts[7].sprite = Resources.Load<Sprite>("Items/8_coat/40822");
        playerModelParts[9].sprite = Resources.Load<Sprite>("Items/10_skirt/41094");
    }

    public void LoadIcon()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].id = DataController.instance.dressupData.dressup_db[i].id;
            icons[i].iconName = DataController.instance.dressupData.dressup_db[i].icon;
        }
    }

    public void SelectOnlyThisIcon(PKIcon icon)
    {
        SoundController.instance.PlaySelectSound();
        Interstitial.instance.ShowInterstitialAd(() =>
       {
           foreach (PKIcon i in icons)
           {
               if (i == icon)
               {
                   i.SelectIcon(true);
               }
               else
               {
                   i.SelectIcon(false);
               }
           }
           selectedIcon = icon;
           LoadItemsForSelectedIcon(selectedIcon.iconName, selectedIcon.id);
           particleEffect.Stop();
       });
    }

    public void LoadItemsForSelectedIcon(string iconName, int iconId)
    {
        var selectedDressupDB = DataController.instance.dressupData.dressup_db.Find(db => db.id == iconId);
        if (selectedDressupDB != null)
        {
            int newItemCount = selectedDressupDB.items.Count;
            for (int i = 0; i < newItemCount; i++)
            {
                PKItem itemScript;

                if (i < items.Count)
                {
                    itemScript = items[i];
                }
                else
                {
                    GameObject item = PKItemsController.instance.GetItem();
                    item.SetActive(true);
                    itemScript = item.GetComponent<PKItem>();
                    items.Add(itemScript);
                }
                string itemId = selectedDressupDB.items[i].id;
                bool adsFromPrefs = PlayerPrefs.GetInt("isAds_" + itemId, selectedDressupDB.items[i].ads ? 1 : 0) == 1;
                itemScript.isAds = adsFromPrefs;
                itemScript.LoadAds(adsFromPrefs);
                itemScript.id = itemId;
                itemScript.LoadImage(iconName);
                itemScript.gameObject.SetActive(true);
            }

            for (int i = newItemCount; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(false);
            }
        }

        if (selectedItemsByIcon.ContainsKey(iconId))
        {
            SelectOnlyThisItem(selectedItemsByIcon[iconId]);
        }
        else if (items.Count > 0)
        {
            SelectOnlyThisItem(items[0].id);
        }
    }

    public void SelectOnlyThisItem(string selectedItemID)
    {
        SoundController.instance.PlaySelectSound();
        if (selectedItemsByIcon.ContainsKey(selectedIcon.id))
        {
            selectedItemsByIcon[selectedIcon.id] = selectedItemID;
        }
        else
        {
            selectedItemsByIcon.Add(selectedIcon.id, selectedItemID);
        }

        foreach (PKItem item in items)
        {
            if (item.id == selectedItemID)
            {
                item.SelectItem(true);
                particleEffect.Play();
                item.LoadImageToModel(selectedIcon.iconName, selectedIcon.id);
            }
            else
            {
                item.SelectItem(false);
            }
        }
    }
    public void DeselectCurrentItem(string deselectedItemID)
    {
        foreach (PKItem item in items)
        {
            if (item.id == deselectedItemID)
            {
                item.SelectItem(false);
                SelectOnlyThisItem(items[0].id);
            }
        }
    }
    public void ResetItems()
    {
        SoundController.instance.PlaySelectSound();
        selectedItemsByIcon.Clear();
        playerModelParts[0].sprite = Resources.Load<Sprite>("Items/1_skin/4011");
        playerModelParts[1].sprite = Resources.Load<Sprite>("Items/2_frontHair/222");
        playerModelParts[2].sprite = Resources.Load<Sprite>("Items/3_backHair/333");
        playerModelParts[3].sprite = Resources.Load<Sprite>("Items/4_eyes/444");
        playerModelParts[4].sprite = Resources.Load<Sprite>("Items/5_eyebrown/555");
        playerModelParts[5].sprite = Resources.Load<Sprite>("Items/6_mouth/666");
        playerModelParts[6].sprite = Resources.Load<Sprite>("Items/7_vest/777");
        playerModelParts[7].sprite = Resources.Load<Sprite>("Items/8_coat/888");
        playerModelParts[7].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        playerModelParts[8].sprite = Resources.Load<Sprite>("Items/9_pants/999");
        playerModelParts[9].sprite = Resources.Load<Sprite>("Items/10_skirt/1010");
        playerModelParts[10].sprite = Resources.Load<Sprite>("Items/11_dress/1111");
        playerModelParts[10].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        playerModelParts[11].sprite = Resources.Load<Sprite>("Items/12_socks/1212");
        playerModelParts[12].sprite = Resources.Load<Sprite>("Items/13_shoes/1313");
        playerModelParts[13].sprite = Resources.Load<Sprite>("Items/14_face acc/1414");
        playerModelParts[14].sprite = Resources.Load<Sprite>("Items/15_head acc/1515");
        playerModelParts[15].sprite = Resources.Load<Sprite>("Items/16_neck acc/1616");
        playerModelParts[16].sprite = Resources.Load<Sprite>("Items/17_bags/1717");
        playerModelParts[17].sprite = Resources.Load<Sprite>("Items/18_wing/1818");
        SelectOnlyThisIcon(icons[0]);
    }

    public void BackMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void PK()
    {

        loading.SetActive(true);
        dressup.SetActive(false);
        player.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        player.transform.position = new Vector3(-2.5f, 2f, 0);
        enemy.SetActive(true);
    }

    public int CalculateTotalAttributes()
    {
        int totalAttributes = 0;

        foreach (var selectedItem in selectedItemsByIcon)
        {
            int iconId = selectedItem.Key;
            string itemId = selectedItem.Value;

            var dressupDB = DataController.instance.dressupData.dressup_db.Find(db => db.id == iconId);
            var item = dressupDB?.items.Find(i => i.id == itemId);
            Topic topic = (Topic)TopicController.instance.currentTopic;
            if (item != null)
            {
                if (topic == Topic.Wedding)
                {
                    totalAttributes += item.elegance;
                }
                else if (topic == Topic.Party)
                {
                    totalAttributes += item.sexy;

                }
                else if (topic == Topic.Casual)
                {
                    totalAttributes += item.cool;
                }
                else if (topic == Topic.Business)
                {
                    totalAttributes += item.polite;
                }
                else if (topic == Topic.Travel)
                {
                    totalAttributes += item.cute;
                }
            }
        }
        return totalAttributes;
    }


    public int RandomItemsForEnemy()
    {
        int totalAttributes = 0;
        Topic topic = (Topic)TopicController.instance.currentTopic;
        foreach (PKIcon icon in icons)
        {
            float randomIndex = Random.Range(0, 1f);
            if (winstreak > 3)
            {
                var dressupDB = DataController.instance.dressupData.dressup_db.Find(db => db.id == icon.id);

                if (dressupDB != null && dressupDB.items.Count > 0)
                {
                    List<Item> itemsData = dressupDB.items;
                    Item randomItemData = itemsData[Random.Range(0, itemsData.Count)];
                    Item randomItem = itemsData.Find(t => t.id == randomItemData.id);

                    if (randomItem != null)
                    {
                        Debug.Log("Random item for enemy: " + randomItem.id);
                        Sprite loadedSprite = Resources.Load<Sprite>("Items/" + icon.iconName + "/" + randomItem.id);
                        if (icon.iconName == "1_skin")
                        {
                            enemyModelParts[icon.id].sprite = loadedSprite;
                        }
                        else if (icon.iconName == "4_eyes")
                        {
                            enemyModelParts[icon.id].sprite = loadedSprite;
                        }
                        else if (randomIndex <= 0.7f)
                        {
                            if (icon.iconName == "8_coat" && playerModelParts[10].sprite != null)
                            {
                                enemyModelParts[10].sprite = null;
                                enemyModelParts[7].sprite = loadedSprite;
                                enemyModelParts[10].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                            }
                            else if (icon.iconName == "11_dress" && enemyModelParts[7].sprite != null)
                            {
                                enemyModelParts[7].sprite = null;
                                enemyModelParts[10].sprite = loadedSprite;
                                enemyModelParts[7].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                            }
                            else if (icon.id < enemyModelParts.Count)
                            {
                                enemyModelParts[icon.id].sprite = loadedSprite;
                            }

                            if (topic == Topic.Wedding)
                            {
                                totalAttributes += randomItem.elegance;
                            }
                            else if (topic == Topic.Party)
                            {
                                totalAttributes += randomItem.sexy;
                            }
                            else if (topic == Topic.Casual)
                            {
                                totalAttributes += randomItem.cool;
                            }
                            else if (topic == Topic.Business)
                            {
                                totalAttributes += randomItem.polite;
                            }
                            else if (topic == Topic.Travel)
                            {
                                totalAttributes += randomItem.cute;
                            }
                        }
                        else
                        {
                            if (icon.id < enemyModelParts.Count)
                            {
                                enemyModelParts[icon.id].sprite = null;
                            }
                        }

                        // Chỉ gán đường viền khi có sprite cho dress hoặc coat
                        if (icon.id < enemyModelParts.Count && enemyModelParts[icon.id].transform.childCount > 0)
                        {
                            if ((icon.iconName == "8_coat" && enemyModelParts[7].sprite != null) ||
                                (icon.iconName == "11_dress" && enemyModelParts[10].sprite != null))
                            {
                                Sprite loadLine = Resources.Load<Sprite>("Items/" + icon.iconName + "/" + randomItem.id + "line");
                                if (loadLine != null)
                                {
                                    enemyModelParts[icon.id].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = loadLine;
                                }
                                else
                                {
                                    enemyModelParts[icon.id].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                                }
                            }
                            else
                            {
                                enemyModelParts[icon.id].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("No items found for icon ID: " + icon.id);
                    continue;
                }
            }
            else
            {
                var dressupDB = DataController.instance.dressupData.dressup_db.Find(db => db.id == icon.id);

                if (dressupDB != null && dressupDB.items.Count > 0)
                {
                    List<Item> itemsData = dressupDB.items;
                    Item randomItemData = itemsData[Random.Range(0, itemsData.Count)];
                    Item randomItem = itemsData.Find(t => t.id == randomItemData.id);

                    if (randomItem != null)
                    {
                        Debug.Log("Random item for enemy: " + randomItem.id);
                        Sprite loadedSprite = Resources.Load<Sprite>("Items/" + icon.iconName + "/" + randomItem.id);
                        if (icon.iconName == "1_skin")
                        {
                            enemyModelParts[icon.id].sprite = loadedSprite;
                        }
                        else if (icon.iconName == "4_eyes")
                        {
                            enemyModelParts[icon.id].sprite = loadedSprite;
                        }
                        else if (randomIndex > 0.7f)
                        {
                            if (icon.iconName == "8_coat" && playerModelParts[10].sprite != null)
                            {
                                enemyModelParts[10].sprite = null;
                                enemyModelParts[7].sprite = loadedSprite;
                                enemyModelParts[10].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                            }
                            else if (icon.iconName == "11_dress" && enemyModelParts[7].sprite != null)
                            {
                                enemyModelParts[7].sprite = null;
                                enemyModelParts[10].sprite = loadedSprite;
                                enemyModelParts[7].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                            }
                            else if (icon.id < enemyModelParts.Count)
                            {
                                enemyModelParts[icon.id].sprite = loadedSprite;
                            }

                            if (topic == Topic.Wedding)
                            {
                                totalAttributes += randomItem.elegance;
                            }
                            else if (topic == Topic.Party)
                            {
                                totalAttributes += randomItem.sexy;
                            }
                            else if (topic == Topic.Casual)
                            {
                                totalAttributes += randomItem.cool;
                            }
                            else if (topic == Topic.Business)
                            {
                                totalAttributes += randomItem.polite;
                            }
                            else if (topic == Topic.Travel)
                            {
                                totalAttributes += randomItem.cute;
                            }
                        }
                        else
                        {
                            if (icon.id < enemyModelParts.Count)
                            {
                                enemyModelParts[icon.id].sprite = null;
                            }
                        }

                        // Chỉ gán đường viền khi có sprite cho dress hoặc coat
                        if (icon.id < enemyModelParts.Count && enemyModelParts[icon.id].transform.childCount > 0)
                        {
                            if ((icon.iconName == "8_coat" && enemyModelParts[7].sprite != null) ||
                                (icon.iconName == "11_dress" && enemyModelParts[10].sprite != null))
                            {
                                Sprite loadLine = Resources.Load<Sprite>("Items/" + icon.iconName + "/" + randomItem.id + "line");
                                if (loadLine != null)
                                {
                                    enemyModelParts[icon.id].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = loadLine;
                                }
                                else
                                {
                                    enemyModelParts[icon.id].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                                }
                            }
                            else
                            {
                                enemyModelParts[icon.id].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("No items found for icon ID: " + icon.id);
                    continue;
                }
            }
        }
        Debug.Log("Total attributes for enemy: " + totalAttributes);
        return totalAttributes;
    }


    public void SaveWinStreak()
    {
        PlayerPrefs.SetInt("Winstreak", winstreak);
    }

    public void LoadWinStreak()
    {
        winstreak = PlayerPrefs.GetInt("Winstreak");
    }

}
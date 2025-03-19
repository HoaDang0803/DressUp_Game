using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public List<Icon> icons;
    public List<ThumbItem> items;
    public List<SpriteRenderer> modelParts;
    private Icon selectedIcon;
    public Dictionary<int, string> selectedItemsByIcon = new Dictionary<int, string>();
    public List<SpriteRenderer> showModelParts;

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
        StartCoroutine(InitializeGame());
        particleEffect.Stop();
    }

    void Start()
    {
        int selectedLanguage = PlayerPrefs.GetInt("Language", 0);
        LanguagesManager.instance.ChangeLanguage(selectedLanguage);
        LanguagesManager.instance.languageCanvas.SetActive(true);

        Time.timeScale = 1f;
        Banner.instance.LoadAd();
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitUntil(() => DataController.instance != null && DataController.instance.dressupData != null);
        yield return new WaitUntil(() => ItemsController.instance != null);
        LoadIcon();
        SelectOnlyThisIcon(icons[0]);
        LoadSelectedItems();
        LoadItemsToMainMenu();
    }

    public void BasicModel()
    {
        selectedItemsByIcon.Clear();
        selectedItemsByIcon[0] = "4011";
        selectedItemsByIcon[1] = "40262";
        selectedItemsByIcon[3] = "40432";
        selectedItemsByIcon[4] = "40526";
        selectedItemsByIcon[5] = "2421";
        selectedItemsByIcon[6] = "4071";
        selectedItemsByIcon[7] = "40822";
        selectedItemsByIcon[9] = "41094";
        modelParts[0].sprite = Resources.Load<Sprite>("Items/1_skin/4011");
        modelParts[1].sprite = Resources.Load<Sprite>("Items/2_frontHair/40262");
        modelParts[3].sprite = Resources.Load<Sprite>("Items/4_eyes/40432");
        modelParts[4].sprite = Resources.Load<Sprite>("Items/5_eyebrown/40526");
        modelParts[5].sprite = Resources.Load<Sprite>("Items/6_mouth/2421");
        modelParts[6].sprite = Resources.Load<Sprite>("Items/7_vest/4071");
        modelParts[7].sprite = Resources.Load<Sprite>("Items/8_coat/40822");
        modelParts[7].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Items/8_coat/40822line");
        modelParts[9].sprite = Resources.Load<Sprite>("Items/10_skirt/41094");
    }

    public void LoadIcon()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].id = DataController.instance.dressupData.dressup_db[i].id;
            icons[i].iconName = DataController.instance.dressupData.dressup_db[i].icon;
        }
    }

    public void SelectOnlyThisIcon(Icon icon)
    {
        SoundController.instance.PlaySelectSound();
        Interstitial.instance.ShowInterstitialAd(() =>
        {
            foreach (Icon i in icons)
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
                ThumbItem itemScript;
                if (i < items.Count)
                {
                    itemScript = items[i];
                }
                else
                {
                    GameObject item = ItemsController.instance.GetItem();
                    item.SetActive(true);
                    itemScript = item.GetComponent<ThumbItem>();
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

        foreach (ThumbItem item in items)
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
        foreach (ThumbItem item in items)
        {
            if (item.id == deselectedItemID)
            {
                item.SelectItem(false);
                SelectOnlyThisItem(items[0].id);
            }
        }
    }

    public void LoadItemsToMainMenu()
    {
        if (selectedItemsByIcon == null || selectedItemsByIcon.Count == 0)
        {
            Debug.Log("No items to load. Returning.");
            return;
        }

        for (int i = 0; i < showModelParts.Count; i++)
        {
            showModelParts[i].sprite = null;
        }

        foreach (SpriteRenderer i in showModelParts)
        {
            if (i.sprite == null && i.transform.childCount > 0)
            {
                i.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            }
        }

        foreach (KeyValuePair<int, string> item in selectedItemsByIcon)
        {
            Debug.Log("Loading item: " + item.Key + " " + item.Value);
            string nameIcon = icons[item.Key].iconName;
            Sprite loadedSprite = Resources.Load<Sprite>("Items/" + nameIcon + "/" + item.Value);
            if (loadedSprite != null)
            {
                showModelParts[item.Key].sprite = loadedSprite;
            }

            Sprite loadLine = Resources.Load<Sprite>("Items/" + nameIcon + "/" + item.Value + "line");
            if (loadLine != null)
            {
                showModelParts[item.Key].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = loadLine;
            }
        }

        SaveSelectedItems();
    }

    private bool isProcessing = false;
    public async void RandomizeItems()
    {
        if (isProcessing) return;

        isProcessing = true;
        SoundController.instance.PlaySelectSound();
        selectedItemsByIcon.Clear();

        foreach (Icon icon in icons)
        {
            SelectOnlyThisIcon(icon);
            List<Item> itemsData = DataController.instance.dressupData.dressup_db.Find(db => db.id == icon.id).items;
            Item randomItem = itemsData[Random.Range(0, itemsData.Count)];
            ThumbItem thumbItem = items.Find(t => t.id == randomItem.id);
            if (thumbItem != null)
            {
                SelectOnlyThisItem(thumbItem.id);
            }
        }
        SelectOnlyThisIcon(icons[0]);
        await Task.Delay(500);
        isProcessing = false;
    }

    public void ResetItems()
    {
        SoundController.instance.PlaySelectSound();
        selectedItemsByIcon.Clear();
        modelParts[0].sprite = Resources.Load<Sprite>("Items/1_skin/4011");
        modelParts[1].sprite = Resources.Load<Sprite>("Items/2_frontHair/222");
        modelParts[2].sprite = Resources.Load<Sprite>("Items/3_backHair/333");
        modelParts[3].sprite = Resources.Load<Sprite>("Items/4_eyes/444");
        modelParts[4].sprite = Resources.Load<Sprite>("Items/5_eyebrown/555");
        modelParts[5].sprite = Resources.Load<Sprite>("Items/6_mouth/666");
        modelParts[6].sprite = Resources.Load<Sprite>("Items/7_vest/777");
        modelParts[7].sprite = Resources.Load<Sprite>("Items/8_coat/888");
        modelParts[7].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        modelParts[8].sprite = Resources.Load<Sprite>("Items/9_pants/999");
        modelParts[9].sprite = Resources.Load<Sprite>("Items/10_skirt/1010");
        modelParts[10].sprite = Resources.Load<Sprite>("Items/11_dress/1111");
        modelParts[10].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        modelParts[11].sprite = Resources.Load<Sprite>("Items/12_socks/1212");
        modelParts[12].sprite = Resources.Load<Sprite>("Items/13_shoes/1313");
        modelParts[13].sprite = Resources.Load<Sprite>("Items/14_face acc/1414");
        modelParts[14].sprite = Resources.Load<Sprite>("Items/15_head acc/1515");
        modelParts[15].sprite = Resources.Load<Sprite>("Items/16_neck acc/1616");
        modelParts[16].sprite = Resources.Load<Sprite>("Items/17_bags/1717");
        modelParts[17].sprite = Resources.Load<Sprite>("Items/18_wing/1818");
        SelectOnlyThisIcon(icons[0]);
    }

    public void SaveSelectedItems()
    {
        if (modelParts[6] == null && modelParts[7] == null && modelParts[8] == null && modelParts[9] == null && modelParts[10] == null)
        {
            return;
        }

        foreach (var entry in selectedItemsByIcon)
        {
            PlayerPrefs.SetString("SelectedItem_" + entry.Key, entry.Value);
        }
        PlayerPrefs.Save();
    }

    public void LoadSelectedItems()
    {
        selectedItemsByIcon.Clear();
        bool hasSavedData = false;

        foreach (var modelPart in modelParts)
        {
            modelPart.sprite = null;
            if (modelPart.transform.childCount > 0)
                modelPart.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }

        foreach (Icon icon in icons)
        {
            string savedItemID = PlayerPrefs.GetString("SelectedItem_" + icon.id, null);
            if (!string.IsNullOrEmpty(savedItemID))
            {
                selectedItemsByIcon[icon.id] = savedItemID;
                hasSavedData = true;
            }
        }

        if (!hasSavedData)
        {
            BasicModel();
            showModelParts[0].sprite = Resources.Load<Sprite>("Items/1_skin/4011");
            showModelParts[1].sprite = Resources.Load<Sprite>("Items/2_frontHair/40262");
            showModelParts[3].sprite = Resources.Load<Sprite>("Items/4_eyes/40432");
            showModelParts[4].sprite = Resources.Load<Sprite>("Items/5_eyebrown/40526");
            showModelParts[5].sprite = Resources.Load<Sprite>("Items/6_mouth/2421");
            showModelParts[6].sprite = Resources.Load<Sprite>("Items/7_vest/4071");
            showModelParts[7].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Items/8_coat/40822line");
            showModelParts[7].sprite = Resources.Load<Sprite>("Items/8_coat/40822");
            showModelParts[9].sprite = Resources.Load<Sprite>("Items/10_skirt/41094");
        }
        else
        {
            foreach (var entry in selectedItemsByIcon)
            {
                string nameIcon = icons[entry.Key].iconName;
                Sprite loadedSprite = Resources.Load<Sprite>("Items/" + nameIcon + "/" + entry.Value);
                if (loadedSprite != null && entry.Key < modelParts.Count)
                {
                    modelParts[entry.Key].sprite = loadedSprite;
                    if (modelParts[entry.Key].transform.childCount > 0)
                    {
                        modelParts[entry.Key].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Items/" + nameIcon + "/" + entry.Value + "line");
                    }
                }
            }
        }
    }
}


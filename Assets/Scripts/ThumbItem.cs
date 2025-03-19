using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ThumbItem : MonoBehaviour
{
    public string id;
    public Image selectedImage;
    public Image itemImage;
    public Sprite selectedSprite;
    public Sprite unselectedSprite;
    public bool isSelected;
    public GameObject adsIcon;
    public bool isAds;
    public string Key { get; internal set; }

    void Start()
    {
        selectedImage = GetComponent<Image>();
        itemImage = transform.Find("item").GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnItemClick);
    }

    public void LoadAds(bool ads)
    {
        adsIcon.SetActive(ads);
    }

    public void LoadImage(string iconName)
    {
        Sprite loadedSprite = Resources.Load<Sprite>("Thumbs/" + iconName + "/" + id);

        if (loadedSprite != null)
        {
            itemImage.sprite = loadedSprite;
        }
        else
        {
            Debug.LogError("Không tìm thấy ảnh cho id: " + id);
        }
    }

    public void LoadImageToModel(string iconName, int iconId)
    {
        Sprite loadedSprite = Resources.Load<Sprite>("Items/" + iconName + "/" + id);
        if (loadedSprite != null)
        {
            if (iconName == "8_coat" && GameController.instance.modelParts[10].sprite != null)
            {
                GameController.instance.modelParts[10].sprite = null;
                GameController.instance.modelParts[7].sprite = loadedSprite;

                GameController.instance.modelParts[10].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                if (GameController.instance.selectedItemsByIcon.ContainsKey(10))
                {
                    GameController.instance.selectedItemsByIcon.Remove(10);
                }
            }
            else if (iconName == "11_dress" && GameController.instance.modelParts[7].sprite != null)
            {
                GameController.instance.modelParts[7].sprite = null;
                GameController.instance.modelParts[10].sprite = loadedSprite;
                GameController.instance.modelParts[7].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                if (GameController.instance.selectedItemsByIcon.ContainsKey(7))
                {
                    GameController.instance.selectedItemsByIcon.Remove(7);
                }
            }
            else
            {
                GameController.instance.modelParts[iconId].sprite = loadedSprite;
            }
        }
        else
        {
            GameController.instance.modelParts[iconId].sprite = null;
        }

        Sprite loadLine = Resources.Load<Sprite>("Items/" + iconName + "/" + id + "line");
        if (loadLine != null)
        {
            GameController.instance.modelParts[iconId].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = loadLine;
        }
        else if (GameController.instance.modelParts[iconId].transform.childCount == 0)
        {
            return;
        }
        else
        {
            GameController.instance.modelParts[iconId].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public void OnItemClick()
    {
        Debug.Log("Item clicked: " + id);

        if (isSelected)
        {
            DeselectItem();
        }
        else
        {
            if (isAds)
            {
                Rewarded.instance.ShowRewardedAd(() =>
                {
                    RewardUser();
                });
            }
            else
            {
                GameController.instance.SelectOnlyThisItem(id);
            }
        }
    }

    private void DeselectItem()
    {
        GameController.instance.DeselectCurrentItem(id);

        isSelected = false;
        selectedImage.sprite = unselectedSprite;
    }

    private void RewardUser()
    {
        GameController.instance.SelectOnlyThisItem(id);
        isAds = false;
        adsIcon.SetActive(false);
        PlayerPrefs.SetInt("isAds_" + id, 0);
        PlayerPrefs.Save();
    }

    public void SelectItem(bool select)
    {
        if (select)
        {
            selectedImage.sprite = selectedSprite;
            isSelected = true;
        }
        else
        {
            selectedImage.sprite = unselectedSprite;
            isSelected = false;
        }
    }
}

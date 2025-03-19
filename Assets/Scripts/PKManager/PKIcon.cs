using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PKIcon : MonoBehaviour
{
    public int id;
    public string iconName;
    public Image iconImage;
    public Sprite unselectedSprite;
    public Sprite selectedSprite;
    public bool isSelected = false;

    void Start()
    {
        iconImage = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnIconClick);
    }

    public void OnIconClick()
    {
        PKController.instance.SelectOnlyThisIcon(this);
    }

    public void SelectIcon(bool select)
    {
        Interstitial.instance.ShowInterstitialAd(() =>
        {
            if (select)
            {
                iconImage.sprite = selectedSprite;
                isSelected = true;
            }
            else
            {
                iconImage.sprite = unselectedSprite;
                isSelected = false;
            }
        });
    }
}

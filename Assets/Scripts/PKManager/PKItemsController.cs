using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PKItemsController : MonoBehaviour
{
    public static PKItemsController instance;
    public GameObject itemPrefabs;
    public List<GameObject> items = new List<GameObject>();
    public GameObject thumbnail;
    public int amountItem;
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
        SpawnItem();
    }

    public void SpawnItem()
    {
        items = new List<GameObject>();
        GameObject item;
        for (int i = 0; i < amountItem; i++)
        {
            item = Instantiate(itemPrefabs);
            item.transform.SetParent(thumbnail.transform, false);
            item.SetActive(false);
            items.Add(item);
        }
    }

    public GameObject GetItem()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i].activeSelf)
            {
                return items[i];
            }
        }
        return null;
    }
}

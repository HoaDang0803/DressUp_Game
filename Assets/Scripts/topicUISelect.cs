using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class topicUISelect : MonoBehaviour
{
    public Sprite topicImage;
    public iTopic topic;

    public void Init(iTopic topic)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = topic.topicSprite;
        this.topic = topic;
        topicImage = topic.topicSprite;
    }
}

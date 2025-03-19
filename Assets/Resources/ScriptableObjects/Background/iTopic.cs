using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Background", order = 1)]
public class iTopic : ScriptableObject
{
    public Topic topic;
    public Sprite topicSprite;
    public string topicName;
    public string topicDescription;

    public static explicit operator Topic(iTopic v)
    {
        return v.topic;
    }
}

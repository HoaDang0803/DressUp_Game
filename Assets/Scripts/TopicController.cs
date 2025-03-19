using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Topic
{
    Wedding,
    Party,
    Casual,
    Business,
    Travel
}

public class TopicController : MonoBehaviour
{
    public static TopicController instance;
    public iTopic currentTopic;
    public List<iTopic> topics = new List<iTopic>();

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
    }

}

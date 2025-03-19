using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class SelectTopic : MonoBehaviour
{
    public GameObject topicSelect;
    public SpriteRenderer topic;
    public Button playButton;
    public topicUISelect[] topicUISelects;
    public TextMeshProUGUI topicName;

    void Awake()
    {
        for (int i = 0; i < topicUISelects.Length; i++)
        {
            topicUISelects[i].Init(TopicController.instance.topics[i]);
        }
    }

    void Start()
    {
        int selectedLanguage = PlayerPrefs.GetInt("Language", 0); // Lấy ngôn ngữ đã chọn
        LanguagesManager.instance.ChangeLanguage(selectedLanguage);
        RandomTopic();
    }

    private void RandomTopic()
    {
        int random = Random.Range(0, topicUISelects.Length);

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < 3; i++)
        {
            foreach (var topic in topicUISelects)
            {
                sequence.AppendCallback(() =>
                {
                    topic.gameObject.SetActive(true);
                    UpdateTopicName(topic.topic.topicName);
                })
                .AppendInterval(0.1f)
                .AppendCallback(() => topic.gameObject.SetActive(false));
            }
        }

        sequence.AppendCallback(() =>
         {
             foreach (var topic in topicUISelects)
             {
                 topic.gameObject.SetActive(false);
             }
             topicUISelects[random].gameObject.SetActive(true);
             UpdateTopicName(topicUISelects[random].topic.topicName);
             TopicController.instance.currentTopic = topicUISelects[random].topic;
         });

        sequence.AppendCallback(() => playButton.gameObject.SetActive(true));
    }

    public void Play()
    {
        LanguagesManager.instance.languageCanvas.SetActive(true);
        UpdateTopicName(TopicController.instance.currentTopic.topicName);
        topic.sprite = TopicController.instance.currentTopic.topicSprite;
        playButton.gameObject.SetActive(false);
        topicSelect.SetActive(false);
        PKController.instance.dressup.SetActive(true);
        PKController.instance.player.SetActive(true);

        // int selectedLanguage = PlayerPrefs.GetInt("Language", 0);
        // LanguagesManager.instance.ChangeLanguage(selectedLanguage);
    }

    private void UpdateTopicName(string key)
    {
        if (LanguagesManager.instance != null)
        {
            topicName.text = LanguagesManager.instance.GetLocalizedString(key);
        }
        else
        {
            Debug.LogWarning("LanguageManager chưa được gắn vào scene!");
        }
    }
}

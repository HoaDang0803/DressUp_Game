using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    private void Awake()
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

    [SerializeField] private AudioSource effectSource;
    public AudioClip selectSound;
    public AudioClip saveSound;
    public AudioClip errorSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    public void PlaySelectSound()
    {
        effectSource.PlayOneShot(selectSound);
    }

    public void PlaySaveSound()
    {
        effectSource.PlayOneShot(saveSound);
    }

    public void PlayErrorSound()
    {
        effectSource.PlayOneShot(errorSound);
    }

    public void PlayWinSound()
    {
        effectSource.PlayOneShot(winSound);
    }

    public void PlayLoseSound()
    {
        effectSource.PlayOneShot(loseSound);
    }
}

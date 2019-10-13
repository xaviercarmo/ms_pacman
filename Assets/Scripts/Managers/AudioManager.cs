using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource PlayerAudioSource;
    public AudioSource FruitAudioSource;
    public AudioSource GhostFrightenedAudioSource;
    public AudioSource GhostEatenAudioSource;

    public AudioClip PlayerEatDotClip;
    public AudioClip PlayerDieClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMuteAllSources();
        }
    }

    public void ToggleMuteAllSources()
    {
        PlayerAudioSource.mute = !PlayerAudioSource.mute;
        FruitAudioSource.mute = !FruitAudioSource.mute;
        GhostFrightenedAudioSource.mute = !GhostFrightenedAudioSource.mute;
        GhostEatenAudioSource.mute = !GhostEatenAudioSource.mute;
    }

    public void StopAllSources()
    {
        PlayerAudioSource.Stop();
        FruitAudioSource.Stop();
        GhostFrightenedAudioSource.Stop();
        GhostEatenAudioSource.Stop();
    }

    public void PauseAllSources()
    {
        PlayerAudioSource.Pause();
        FruitAudioSource.Pause();
        GhostFrightenedAudioSource.Pause();
        GhostEatenAudioSource.Pause();
    }

    public void ResumeAllSources()
    {
        PlayerAudioSource.UnPause();
        FruitAudioSource.UnPause();
        GhostFrightenedAudioSource.UnPause();
        GhostEatenAudioSource.UnPause();
    }
}

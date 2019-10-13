using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameLevel
{
    MainMenu,
    OriginalLevel,
    NewLevel
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //finish singleton pattern so multiple aren't made when coming back to main menu

    public static GameLevel CurrentGameLevel { get; private set; } = GameLevel.MainMenu;
    public AudioSource BackgroundMusic;

    float maxBackgroundMusicVolume = 0.3f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        StartCoroutine(FadeInBackgroundMusic());
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            BackgroundMusic.mute = !BackgroundMusic.mute;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentGameLevel == GameLevel.OriginalLevel)
            {
                SetLevel(GameLevel.MainMenu);
            }
        }
    }

    void StartGame()
    {
    }

    public static void SetLevel(GameLevel newGameLevel)
    {
        if (newGameLevel != CurrentGameLevel)
        {
            SceneManager.UnloadSceneAsync((int)CurrentGameLevel);
            SceneManager.LoadSceneAsync((int)newGameLevel);
            CurrentGameLevel = newGameLevel;
        }
    }

    IEnumerator FadeInBackgroundMusic()
    {
        BackgroundMusic.volume = 0;
        BackgroundMusic.Play();
        while (BackgroundMusic.volume < maxBackgroundMusicVolume)
        {
            BackgroundMusic.volume += 0.1f * Time.deltaTime;
            yield return null;
        }

        BackgroundMusic.volume = maxBackgroundMusicVolume;
    }
}

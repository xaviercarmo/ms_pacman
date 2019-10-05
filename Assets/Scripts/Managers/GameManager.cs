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
    public static GameLevel CurrentGameLevel { get; private set; }
    public static Grid LevelGrid { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        //later, load the main menu rather than this level
        CurrentGameLevel = GameLevel.OriginalLevel;
        SceneManager.LoadScene((int)CurrentGameLevel);
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
}

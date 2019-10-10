﻿using System.Collections;
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

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        //later, load the main menu rather than this level
        CurrentGameLevel = GameLevel.OriginalLevel;
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            //Time.timeScale = 0;
            //Invoke("StartGame", 5f);
        };
        SceneManager.LoadScene((int)CurrentGameLevel);
    }

    void Start()
    {
    }

    void StartGame()
    {
        Debug.Log("Fuck you");
        Time.timeScale = 1f;
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

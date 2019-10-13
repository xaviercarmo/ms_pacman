using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OriginalLevelManager : MonoBehaviour
{
    public static OriginalLevelManager Instance { get; private set; }

    public Text StatusText;

    public bool GameSuspended { get; private set; } = true;
    float gameResetTime = 3f;

    bool gameStarted = false;
    bool canPause = false;

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
        if (!gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameStarted = true;
                canPause = true;
                GameSuspended = false;
                PlayerManager.Instance.Points = 0;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.P) && canPause)
            {
                if (GameSuspended)
                {
                    StatusText.text = string.Empty;
                    GameSuspended = false;
                    AudioManager.Instance.ResumeAllSources();
                }
                else
                {
                    StatusText.text = "Paused";
                    GameSuspended = true;
                    AudioManager.Instance.PauseAllSources();
                }
            }

            if (PlayerManager.Instance.DotsEaten == 152)
            {
                StatusText.text = "You  Won!";
                AudioManager.Instance.StopAllSources();
                GameSuspended = true;
            }
            else if (PlayerManager.Instance.Lives <= 0)
            {
                StatusText.text = "Game  Over...";
                AudioManager.Instance.StopAllSources();
                GameSuspended = true;
            }
        }
    }

    public void ResetLevel()
    {
        AudioManager.Instance.StopAllSources();

        GameSuspended = true;
        canPause = false;
        Instance.Invoke("ResumeGame", gameResetTime);

        PlayerManager.Instance.ResetState();
        GhostManager.Instance.ResetState();
        PlayerInputManager.ResetState();
    }

    void ResumeGame()
    {
        GameSuspended = false;
        canPause = true;
        PlayerManager.Instance.MovementHandler.ResumeAfterReset();
    }
}

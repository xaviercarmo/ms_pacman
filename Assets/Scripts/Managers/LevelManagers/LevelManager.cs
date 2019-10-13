using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Text StatusText;

    public bool GameSuspended { get; protected set; } = true;
    public bool ScrollingMode = false;

    protected float gameResetTime = 3f;

    protected bool gameStarted = false;
    protected bool canPause = false;

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
        InvokeRepeating("ToggleStartText", 0.4f, 0.4f);
    }

    void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CancelInvoke("ToggleStartText");
                PlayerManager.Instance.ScoreText.enabled = true;
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

            EndConditions();
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

    void ToggleStartText()
    {
        PlayerManager.Instance.ScoreText.enabled = !PlayerManager.Instance.ScoreText.enabled;
    }

    protected abstract void EndConditions();
}

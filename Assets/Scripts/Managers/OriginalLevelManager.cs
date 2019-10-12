using UnityEngine;
using System.Collections;

public class OriginalLevelManager : MonoBehaviour
{
    public static OriginalLevelManager Instance { get; private set; }

    public bool GameSuspended { get; private set; } = true;
    public float GameResetTime = 3;
    public GhostManager GhostManager;

    bool gameStarted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            GhostManager = GetComponent<GhostManager>();
        }
    }

    void Start()
    {
        //Instance.Invoke("ResumeGame", GameResetTime);
    }

    void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameStarted = true;
                GameSuspended = false;
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameSuspended = !GameSuspended;
        }

        if (PlayerManager.Instance.DotsEaten == 152)
        {
            //game over
            GameSuspended = true;
        }
        else if (PlayerManager.Instance.Lives <= 0)
        {
            //game over
            GameSuspended = true;
        }
    }

    public void ResetLevel()
    {
        GameSuspended = true;
        Instance.Invoke("ResumeGame", GameResetTime);

        PlayerManager.Instance.ResetState();
        GhostManager.ResetState();
        PlayerInputManager.ResetState();
    }

    void ResumeGame()
    {
        GameSuspended = false;
        PlayerManager.Instance.MovementHandler.ResumeAfterReset();
    }
}

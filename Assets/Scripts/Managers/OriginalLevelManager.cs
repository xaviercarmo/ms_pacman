using UnityEngine;
using System.Collections;

public class OriginalLevelManager : MonoBehaviour
{
    public static OriginalLevelManager Instance { get; private set; }

    public bool GameSuspended { get; private set; } = false;
    public float GameResetTime = 5;
    public GhostManager GhostManager;

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
    }

    void Update()
    {
        if (PlayerManager.Instance.DotsEaten == 152)
        {
            Time.timeScale = 0;
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

using UnityEngine;
using System.Collections;

public class OriginalLevelManager : MonoBehaviour
{
    public static OriginalLevelManager Instance { get; private set; }

    public bool GameResetting { get; private set; } = false;
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
    }

    public void ResetLevel()
    {
        GameResetting = true;
        Instance.Invoke("ResumeGame", 5);

        PlayerManager.ResetState();
        GhostManager.ResetState();
        PlayerInputManager.ResetState();
    }

    void ResumeGame()
    {
        GameResetting = false;
        PlayerManager.MovementHandler.ResumeAfterReset();
    }
}

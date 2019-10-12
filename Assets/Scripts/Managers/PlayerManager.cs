using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public SpriteRenderer Renderer;
    public Animator Animator;
    public Tweener Tweener;
    public PlayerMovement MovementHandler;

    public Grid LevelGrid;
    public Tilemap WallsTilemap;
    public Tilemap ConsumablesTilemap;
    public Tilemap HorizontalPortalsTilemap;

    public Vector3 HomeWorldPos = Vector3.zero;
    public int DotsEaten = 0;

    int lives = 3;

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
    }

    public void ResetState()
    {
        MovementHandler.ResetState();
        lives--;
    }
}

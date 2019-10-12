using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public SpriteRenderer Renderer;
    public Animator Animator;
    public Tweener Tweener;
    public PlayerMovement MovementHandler;
    public Text ScoreText;
    public Image[] LifeImages;

    public Grid LevelGrid;
    public Tilemap WallsTilemap;
    public Tilemap DotsTilemap;
    public Tilemap HorizontalPortalsTilemap;
    public Tilemap DownBlockersTilemap;

    public Vector3 HomeWorldPos = Vector3.zero;

    int points = 0;
    public int Points
    {
        get => points;
        set
        {
            ScoreText.text = value.ToString();
            points = value;
        }
    }

    public int DotsEaten = 0;
    public int Lives = 3;

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
        Lives--;
        LifeImages[Lives].enabled = false;
    }
}

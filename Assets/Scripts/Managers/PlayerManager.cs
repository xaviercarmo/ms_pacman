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
    public Image[] HealthBarImages;
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
    public float Health = 100;

    float totalHealth = 100;

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
        if (HealthBarImages != null && HealthBarImages[0].IsActive())
        {
            var healthWidth = Mathf.Max(25 + (218 - 25) * Health / totalHealth, 25);
            HealthBarImages[0].rectTransform.sizeDelta = new Vector2(healthWidth, HealthBarImages[0].rectTransform.sizeDelta.y);
        }
    }

    public void PlayerDieBehaviour()
    {
        AudioManager.Instance.PlayerAudioSource.clip = AudioManager.Instance.PlayerDieClip;
        AudioManager.Instance.PlayerAudioSource.loop = false;
        AudioManager.Instance.PlayerAudioSource.UnPause();
        AudioManager.Instance.PlayerAudioSource.Play();
        Animator.SetTrigger("Died");
    }

    public void ResetState()
    {
        PlayerDieBehaviour();

        if (Lives > 0)
        {
            LifeImages[--Lives].enabled = false;
        }

        MovementHandler.ResetState();
    }
}

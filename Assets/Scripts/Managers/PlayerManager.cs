using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public static GameObject Player;
    public static SpriteRenderer Renderer;
    public static Animator Animator;
    public static Tweener Tweener;
    public static PlayerMovement MovementHandler;

    public static Vector3 HomeWorldPos = Vector3.zero;
    public static int DotsEaten = 0;

    static int lives = 3;

    void Awake()
    {
        Player = gameObject;
        Renderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        Tweener = GetComponent<Tweener>();
        MovementHandler = GetComponent<PlayerMovement>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public static void ResetState()
    {
        MovementHandler.ResetState();
        lives--;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance { get; private set; }

    public GameObject RedGhostPrefab;
    public GameObject BlueGhostPrefab;
    public GameObject PinkGhostPrefab;
    public GameObject OrangeGhostPrefab;

    public Grid LevelGrid;
    public Tilemap WallsTilemap;
    public Tilemap UpBlockersTilemap;
    public Tilemap DownBlockersTilemap;
    public Tilemap HorizontalPortalsTilemap;
    public Tilemap GhostSlowersTilemap;

    public static Vector3 RedGhostStartWorldPos = new Vector3(0, 4);
    public static Vector3Int RedGhostStartCellPos;
    public static Vector3 BlueGhostStartWorldPos = new Vector3(-2, 2);
    public static Vector3 PinkGhostStartWorldPos = new Vector3(0, 2);
    public static Vector3 OrangeGhostStartWorldPos = new Vector3(2, 2);

    public static int DotsEaten = 0;

    List<GhostBehaviour> ghostBehaviours;

    float modeTime = 0;
    Queue<(GhostMode, float)> modeQueue;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            ghostBehaviours = new List<GhostBehaviour>();
            modeQueue = new Queue<(GhostMode, float)>
            (
                new (GhostMode, float)[]
                {
                (GhostMode.Scatter, 0f),
                (GhostMode.Chase, 7f),
                (GhostMode.Scatter, 200f),
                (GhostMode.Chase, 5f),
                (GhostMode.Scatter, 20f),
                (GhostMode.Chase, 5f),
                (GhostMode.Scatter, float.PositiveInfinity),
                (GhostMode.Chase, -1)
                }
            );
        }
    }

    void Start()
    {
        RedGhostStartCellPos = LevelGrid.WorldToCell(RedGhostStartWorldPos);

        var playerGameObject = GameObject.FindWithTag("Player");
        SpawnGhost(RedGhostPrefab, RedGhostStartWorldPos, new RedGhostBehaviour());
        SpawnGhost(BlueGhostPrefab, BlueGhostStartWorldPos, new BlueGhostBehaviour(ghostBehaviours[0].MovementHandler));
        SpawnGhost(PinkGhostPrefab, PinkGhostStartWorldPos, new PinkGhostBehaviour());
        SpawnGhost(OrangeGhostPrefab, OrangeGhostStartWorldPos, new OrangeGhostBehaviour());

        var upBlockersRenderer = UpBlockersTilemap.GetComponent<TilemapRenderer>();
        upBlockersRenderer.enabled = false;

        var downBlockersRenderer = DownBlockersTilemap.GetComponent<TilemapRenderer>();
        downBlockersRenderer.enabled = false;

        var ghostSlowersRenderer = GhostSlowersTilemap.GetComponent<TilemapRenderer>();
        ghostSlowersRenderer.enabled = false;
    }

    void Update()
    {
        if (OriginalLevelManager.Instance.GameSuspended) { return; }

        modeTime += Time.deltaTime;
        if (modeTime >= modeQueue.Peek().Item2)
        {
            modeTime = 0;
            ChangeGhostMode(modeQueue.Dequeue().Item1);
        }
    }

    void SpawnGhost(GameObject prefab, Vector3 worldPos, GhostBehaviour behaviour)
    {
        var ghostGameObject = Instantiate(prefab, worldPos, Quaternion.identity);
        ghostGameObject.name = behaviour.GetType().Name;

        var ghostMovementHandler = ghostGameObject.GetComponent<GhostMovement>();
        ghostMovementHandler.Behaviour = behaviour;
        ghostMovementHandler.InitialWorldPos = worldPos;

        behaviour.MovementHandler = ghostMovementHandler;

        ghostBehaviours.Add(behaviour);
    }

    void ChangeGhostMode(GhostMode mode)
    {
        ghostBehaviours.ForEach(behaviour => behaviour.SetMode(mode));
    }

    public void ResetState()
    {
        ghostBehaviours.ForEach(behaviour => behaviour.ResetState());
    }
}

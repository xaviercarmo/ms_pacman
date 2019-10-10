using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostManager : MonoBehaviour
{
    public GameObject RedGhostPrefab;
    public GameObject BlueGhostPrefab;
    public GameObject PinkGhostPrefab;
    public GameObject OrangeGhostPrefab;
    public Grid levelGrid;
    public Tilemap wallsTilemap;
    public Tilemap upBlockersTilemap;
    public Tilemap downBlockersTilemap;

    public static int DotsEaten = 0;

    List<GhostBehaviour> ghostBehaviours;
    float modeTime = 0;
    Queue<(GhostMode, float)> modeQueue;

    private void Awake()
    {
        ghostBehaviours = new List<GhostBehaviour>();
        modeQueue = new Queue<(GhostMode, float)>
        (
            new (GhostMode, float)[]
            {
                (GhostMode.Scatter, 0f),
                (GhostMode.Chase, 7f),
                (GhostMode.Scatter, 20f),
                (GhostMode.Chase, 5f),
                (GhostMode.Scatter, 20f),
                (GhostMode.Chase, 5f),
                (GhostMode.Scatter, float.PositiveInfinity),
                (GhostMode.Chase, -1)
            }
        );
    }

    void Start()
    {
        var playerGameObject = GameObject.FindWithTag("Player");
        SpawnGhost(RedGhostPrefab, new Vector3(0, 4), new RedGhostBehaviour(playerGameObject, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap));
        SpawnGhost(OrangeGhostPrefab, new Vector3(-2, 2), new OrangeGhostBehaviour(playerGameObject, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap));
        SpawnGhost(PinkGhostPrefab, new Vector3(0, 2), new PinkGhostBehaviour(playerGameObject, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap));
        SpawnGhost(BlueGhostPrefab, new Vector3(2, 2), new BlueGhostBehaviour(playerGameObject, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap));

        var upBlockersRenderer = upBlockersTilemap.GetComponent<TilemapRenderer>();
        upBlockersRenderer.enabled = false;

        var downBlockersRenderer = downBlockersTilemap.GetComponent<TilemapRenderer>();
        downBlockersRenderer.enabled = false;
    }

    void Update()
    {
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
        ghostMovementHandler.LevelGrid = levelGrid;
        ghostMovementHandler.WallsTilemap = wallsTilemap;

        ghostMovementHandler.Behaviour = behaviour;
        behaviour.MovementHandler = ghostMovementHandler;

        ghostBehaviours.Add(behaviour);
    }

    void ChangeGhostMode(GhostMode mode)
    {
        ghostBehaviours.ForEach(behaviour => behaviour.SetMode(mode));
    }
}

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

    List<GhostBehaviour> ghostBehaviours;

    private void Awake()
    {
        ghostBehaviours = new List<GhostBehaviour>();
    }

    void Start()
    {
        SpawnGhost(RedGhostPrefab, new Vector3(0, 4), new RedGhostBehaviour(GameObject.FindWithTag("Player"), levelGrid, wallsTilemap, upBlockersTilemap));
        SpawnGhost(OrangeGhostPrefab, new Vector3(-2, 2), new OrangeGhostBehaviour());
        SpawnGhost(PinkGhostPrefab, new Vector3(0, 2), new PinkGhostBehaviour());
        SpawnGhost(BlueGhostPrefab, new Vector3(2, 2), new BlueGhostBehaviour());

        var upBlockersRenderer = upBlockersTilemap.GetComponent<TilemapRenderer>();
        upBlockersRenderer.enabled = false;

        ChangeGhostMode(GhostMode.Chase);
    }

    void Update()
    {
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

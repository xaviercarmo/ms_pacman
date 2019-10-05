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

    List<IGhostBehaviour> ghostBehaviours;

    void Start()
    {
        ghostBehaviours = new List<IGhostBehaviour>();

        SpawnGhost(OrangeGhostPrefab, new Vector3(-2, 2), new OrangeGhostBehaviour());
        SpawnGhost(PinkGhostPrefab, new Vector3(0, 2), new PinkGhostBehaviour());
        SpawnGhost(RedGhostPrefab, new Vector3(0, 4), new RedGhostBehaviour(GameObject.FindWithTag("Player"), levelGrid, wallsTilemap));
        SpawnGhost(BlueGhostPrefab, new Vector3(2, 2), new BlueGhostBehaviour());
    }

    void Update()
    {

    }

    void SpawnGhost(GameObject prefab, Vector3 worldPos, IGhostBehaviour behaviour)
    {
        var ghostMovement = Instantiate(prefab, worldPos, Quaternion.identity).GetComponent<GhostMovement>();
        ghostMovement.levelGrid = levelGrid;
        ghostMovement.wallsTilemap = wallsTilemap;
        ghostMovement.behaviour = behaviour;

        ghostBehaviours.Add(behaviour);
    }
}

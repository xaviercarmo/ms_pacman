using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public enum GhostMode
{
    Idle,
    Chase,
    Scatter,
    Frightened
}

public abstract class GhostBehaviour
{
    //Public fields/properties
    public abstract Vector3Int GetNextTargetCellPos(Vector3Int previousCellPos, Vector3Int currentCellPos);
    public GhostMovement MovementHandler;

    //Protected field/properties
    protected GhostMode mode = GhostMode.Idle;

    protected GameObject player;
    protected Grid levelGrid;
    protected Tilemap wallsTilemap;
    protected Tilemap upBlockersTilemap;

    protected Vector3Int scatterGoalCellPos;

    //Public Methods
    public void SetMode(GhostMode mode)
    {
        if (this.mode == GhostMode.Chase || this.mode == GhostMode.Scatter)
        {
            MovementHandler.ShouldReverseMovement = true;
        }

        this.mode = mode;
    }
}
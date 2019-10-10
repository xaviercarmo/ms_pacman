using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class BlueGhostBehaviour : GhostBehaviour
{
    public BlueGhostBehaviour(GameObject player, Grid levelGrid, Tilemap wallsTilemap, Tilemap upBlockersTilemap, Tilemap downBlockersTilemap)
        : base(player, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap)
    {
        scatterGoalCellPos = new Vector3Int(wallsTilemap.cellBounds.xMax, wallsTilemap.cellBounds.yMin, 0);
    }

    protected override Vector3Int GetGoalCell()
    {
        var playerCellPos = levelGrid.WorldToCell(player.transform.position);
        return playerCellPos;
    }
}

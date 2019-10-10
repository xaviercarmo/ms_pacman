using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class PinkGhostBehaviour : GhostBehaviour
{
    public PinkGhostBehaviour(GameObject player, Grid levelGrid, Tilemap wallsTilemap, Tilemap upBlockersTilemap, Tilemap downBlockersTilemap)
        : base(player, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap)
    {
        scatterGoalCellPos = new Vector3Int(wallsTilemap.cellBounds.xMin, wallsTilemap.cellBounds.yMax, 0);
    }

    //Get the end goal cell the pink ghost wants to reach
    protected override Vector3Int GetGoalCell()
    {
        var playerCellPos = levelGrid.WorldToCell(player.transform.position);
        return playerCellPos;
        //up to this one
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RedGhostBehaviour : GhostBehaviour
{
    public RedGhostBehaviour(GameObject player, Grid levelGrid, Tilemap wallsTilemap, Tilemap upBlockersTilemap, Tilemap downBlockersTilemap)
        : base(player, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap)
    {
        scatterGoalCellPos = new Vector3Int(wallsTilemap.cellBounds.xMax, wallsTilemap.cellBounds.yMax, 0);
    }

    //Get the end goal cell the red ghost wants to reach
    protected override Vector3Int GetGoalCell() => levelGrid.WorldToCell(Player.transform.position);

    protected override void AdditionalResetBehaviour()
    {
    }
}

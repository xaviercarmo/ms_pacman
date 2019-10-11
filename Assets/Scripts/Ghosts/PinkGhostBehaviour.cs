using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class PinkGhostBehaviour : GhostBehaviour
{
    public PinkGhostBehaviour(GameObject player, Grid levelGrid, Tilemap wallsTilemap, Tilemap upBlockersTilemap, Tilemap downBlockersTilemap)
        : base(player, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap)
    {
        scatterGoalCellPos = new Vector3Int(wallsTilemap.cellBounds.xMin, wallsTilemap.cellBounds.yMax, 0);
        dotsBeforeRelease = 10;
        secondsBeforeRelease = 5;
    }

    //Returns the cell 3 spots ahead of ms pacman's current cell
    protected override Vector3Int GetGoalCell()
        => playerMovement.CurrentCellPos + (playerMovement.TargetCellPos - playerMovement.CurrentCellPos) * 3;

    protected override void AdditionalResetBehaviour()
    {
        secondsBeforeRelease = 3;
    }
}

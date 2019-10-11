using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class OrangeGhostBehaviour : GhostBehaviour
{
    public OrangeGhostBehaviour(GameObject player, Grid levelGrid, Tilemap wallsTilemap, Tilemap upBlockersTilemap, Tilemap downBlockersTilemap)
        : base(player, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap)
    {
        scatterGoalCellPos = new Vector3Int(wallsTilemap.cellBounds.xMin, wallsTilemap.cellBounds.yMin, 0);
        dotsBeforeRelease = 45;
        secondsBeforeRelease = 30;
    }

    protected override Vector3Int GetGoalCell()
    {
        if (Vector3Int.Distance(playerMovement.CurrentCellPos, MovementHandler.CurrentCellPos) < 8)
        {
            return scatterGoalCellPos;
        }
        else
        {
            return playerMovement.CurrentCellPos;
        }
    }

    protected override void AdditionalResetBehaviour()
    {
        secondsBeforeRelease = 9;
    }
}

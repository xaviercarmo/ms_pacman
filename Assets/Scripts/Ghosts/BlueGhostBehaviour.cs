using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class BlueGhostBehaviour : GhostBehaviour
{
    GhostMovement redGhostMovementHandler;

    public BlueGhostBehaviour(GameObject player, Grid levelGrid, Tilemap wallsTilemap, Tilemap upBlockersTilemap, Tilemap downBlockersTilemap, GhostMovement redGhostMovementHandler)
        : base(player, levelGrid, wallsTilemap, upBlockersTilemap, downBlockersTilemap)
    {
        this.redGhostMovementHandler = redGhostMovementHandler;

        scatterGoalCellPos = new Vector3Int(wallsTilemap.cellBounds.xMax, wallsTilemap.cellBounds.yMin, 0);
        dotsBeforeRelease = 30;
        secondsBeforeRelease = 15;
    }

    //Gets a vector from red ghost to cell 2 ahead of ms pacman, doubles that vector, and returns the cell the vector ends on
    protected override Vector3Int GetGoalCell()
    {
        var targetCellPos = playerMovement.CurrentCellPos + (playerMovement.TargetCellPos - playerMovement.CurrentCellPos) * 2;
        var vector = (targetCellPos - redGhostMovementHandler.CurrentCellPos) * 2;
        return levelGrid.WorldToCell(redGhostMovementHandler.CurrentCellPos + vector);
    }
}

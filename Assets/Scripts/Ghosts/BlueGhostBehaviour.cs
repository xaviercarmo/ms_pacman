using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class BlueGhostBehaviour : GhostBehaviour
{
    GhostMovement redGhostMovementHandler;

    public BlueGhostBehaviour(GhostMovement redGhostMovementHandler)
    {
        this.redGhostMovementHandler = redGhostMovementHandler;

        scatterGoalCellPos = new Vector3Int(GhostManager.Instance.WallsTilemap.cellBounds.xMax, GhostManager.Instance.WallsTilemap.cellBounds.yMin, 0);
        dotsBeforeRelease = 30;
        SecondsBeforeRelease = 15;
    }

    //Gets a vector from red ghost to cell 2 ahead of ms pacman, doubles that vector, and returns the cell the vector ends on
    protected override Vector3Int GetGoalCell()
    {
        var targetCellPos = PlayerManager.Instance.MovementHandler.CurrentCellPos + (PlayerManager.Instance.MovementHandler.TargetCellPos - PlayerManager.Instance.MovementHandler.CurrentCellPos) * 2;
        var vector = (targetCellPos - redGhostMovementHandler.CurrentCellPos) * 2;
        return GhostManager.Instance.LevelGrid.WorldToCell(redGhostMovementHandler.CurrentCellPos + vector);
    }

    protected override void AdditionalResetBehaviour()
    {
        SecondsBeforeRelease = 6;
    }
}

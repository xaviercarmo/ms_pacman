using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class PinkGhostBehaviour : GhostBehaviour
{
    public PinkGhostBehaviour()
    {
        scatterGoalCellPos = new Vector3Int(GhostManager.Instance.WallsTilemap.cellBounds.xMin, GhostManager.Instance.WallsTilemap.cellBounds.yMax, 0);
        dotsBeforeRelease = 10;
        secondsBeforeRelease = 5;
    }

    //Returns the cell 3 spots ahead of ms pacman's current cell
    protected override Vector3Int GetGoalCell()
        => PlayerManager.Instance.MovementHandler.CurrentCellPos + (PlayerManager.Instance.MovementHandler.TargetCellPos - PlayerManager.Instance.MovementHandler.CurrentCellPos) * 3;

    protected override void AdditionalResetBehaviour()
    {
        secondsBeforeRelease = 3;
    }
}

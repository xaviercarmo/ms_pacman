using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class OrangeGhostBehaviour : GhostBehaviour
{
    public OrangeGhostBehaviour()
    {
        scatterGoalCellPos = new Vector3Int(GhostManager.Instance.WallsTilemap.cellBounds.xMin, GhostManager.Instance.WallsTilemap.cellBounds.yMin, 0);
        dotsBeforeRelease = 45;
        SecondsBeforeRelease = 30;
    }

    protected override Vector3Int GetGoalCell()
    {
        if (Vector3Int.Distance(PlayerManager.Instance.MovementHandler.CurrentCellPos, MovementHandler.CurrentCellPos) < 8)
        {
            return scatterGoalCellPos;
        }
        else
        {
            return PlayerManager.Instance.MovementHandler.CurrentCellPos;
        }
    }

    protected override void AdditionalResetBehaviour()
    {
        SecondsBeforeRelease = 9;
    }
}

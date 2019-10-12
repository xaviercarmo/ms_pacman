using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RedGhostBehaviour : GhostBehaviour
{
    public RedGhostBehaviour()
    {
        scatterGoalCellPos = new Vector3Int(GhostManager.Instance.WallsTilemap.cellBounds.xMax, GhostManager.Instance.WallsTilemap.cellBounds.yMax, 0);
    }

    //Get the end goal cell the red ghost wants to reach
    protected override Vector3Int GetGoalCell() => GhostManager.Instance.LevelGrid.WorldToCell(PlayerManager.Instance.gameObject.transform.position);

    protected override void AdditionalResetBehaviour()
    {
    }
}

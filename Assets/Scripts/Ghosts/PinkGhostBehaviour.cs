using UnityEngine;
using System.Collections;

public class PinkGhostBehaviour : GhostBehaviour
{
    public override Vector3Int GetNextTargetCellPos(Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        return new Vector3Int();
    }
}

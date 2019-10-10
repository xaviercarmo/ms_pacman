using UnityEngine;
using System.Collections;

public class BlueGhostBehaviour : GhostBehaviour
{
    public override Vector3Int GetNextTargetCellPos(Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        return new Vector3Int();
    }
}

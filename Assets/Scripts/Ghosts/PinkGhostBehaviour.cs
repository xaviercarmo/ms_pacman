﻿using UnityEngine;
using System.Collections;

public class PinkGhostBehaviour : IGhostBehaviour
{
    public GhostMode Mode { get; set; }

    public Vector3Int GetNextTargetCellPos(Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        return new Vector3Int();
    }
}

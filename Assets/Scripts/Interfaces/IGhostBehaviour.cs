using UnityEngine;
using UnityEditor;

public enum GhostMode
{
    Idle,
    Chase,
    Scatter,
    Frightened
}

public interface IGhostBehaviour
{
    GhostMode Mode { get; set; }
    Vector3Int GetNextTargetCellPos(Vector3Int previousCellPos, Vector3Int currentCellPos);
}
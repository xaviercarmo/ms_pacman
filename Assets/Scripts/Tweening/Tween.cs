using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    public Transform Target;
    public Vector3 StartPos;
    public Vector3 EndPos;
    public float StartTime;
    public float Duration;

    public Tween(Transform target, Vector3 startPos, Vector3 endPos, float startTime, float duration)
    {
        Target = target;
        StartPos = startPos;
        EndPos = endPos;
        StartTime = startTime;
        Duration = duration;
    }

    public virtual void Reverse()
    {
        (StartPos, EndPos) = (EndPos, StartPos);
        StartTime = Time.time - (StartTime + Duration - Time.time);
    }
}

//Specialisation of tween that is designed to tween between cell positions
//allows for better tweening on grids that are moving around
public class CellTween : Tween
{
    public Vector3Int StartCellPos;
    public Vector3Int EndCellPos;

    public CellTween(Transform target, Vector3Int startCellPos, Vector3Int endCellPos, float startTime, float duration)
        : base(target, PlayerManager.Instance.LevelGrid.GetCellCenterWorld(startCellPos), PlayerManager.Instance.LevelGrid.GetCellCenterWorld(startCellPos), startTime, duration)
    {
        StartCellPos = startCellPos;
        EndCellPos = endCellPos;
    }

    public override void Reverse()
    {
        (StartCellPos, EndCellPos) = (EndCellPos, StartCellPos);
        StartTime = Time.time - (StartTime + Duration - Time.time);
        UpdateWorldPositions();
    }

    public void UpdateWorldPositions()
    {
        StartPos = PlayerManager.Instance.LevelGrid.GetCellCenterWorld(StartCellPos);
        EndPos = PlayerManager.Instance.LevelGrid.GetCellCenterWorld(EndCellPos);
    }
}
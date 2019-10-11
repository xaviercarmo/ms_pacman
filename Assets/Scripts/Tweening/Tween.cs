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

    public void Reverse()
    {
        (StartPos, EndPos) = (EndPos, StartPos);
        StartTime = Time.time - (StartTime + Duration - Time.time);
    }
}

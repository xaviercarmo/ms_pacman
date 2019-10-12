using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GhostMode
{
    Idle,
    Chase,
    Scatter,
    Frightened,
    RunningHome
}

public abstract class GhostBehaviour
{
    //Public fields/properties
    public GhostMovement MovementHandler;
    public GhostMode Mode { get; protected set; } = GhostMode.Idle;

    //Protected field/properties
    protected float secondsBeforeRelease = 0;
    protected Vector3Int scatterGoalCellPos;
    protected int dotsBeforeRelease = 0;

    //Private fields/properties
    bool released = false;

    //Sets the ghost mode and reverses movement when previous mode was chase or scatter
    public void SetMode(GhostMode mode)
    {
        if (Mode == GhostMode.Chase || Mode == GhostMode.Scatter)
        {
            MovementHandler.ShouldReverseMovement = true;
        }

        Mode = mode;
    }

    public void DelayRelease(float secondsBeforeRelease)
    {
        released = false;
        this.secondsBeforeRelease = secondsBeforeRelease;
    }

    //Gets the next target cell based on ghost mode, returns early if release conditions not met
    public Vector3Int GetNextTargetCellPos(Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        if (PlayerManager.Instance.DotsEaten < dotsBeforeRelease && (secondsBeforeRelease -= Time.deltaTime) >= 0)
        {
            return currentCellPos;
        }
        else if (!released)
        {
            secondsBeforeRelease = 0;

            if (MovementHandler.CurrentCellPos != GhostManager.RedGhostStartCellPos)
            {
                return GetNextTargetCellTowardsGoal(GhostManager.RedGhostStartCellPos, currentCellPos, currentCellPos);
            }
            else
            {
                released = true;
            }
        }

        if (Mode == GhostMode.RunningHome && currentCellPos == GhostManager.GhostHomeCenter)
        {
            Mode = GhostMode.Chase;
        }

        switch (Mode)
        {
            case GhostMode.Chase:
                return GetNextTargetCellTowardsGoal(GetGoalCell(), previousCellPos, currentCellPos);
            case GhostMode.Scatter:
                return GetNextTargetCellTowardsGoal(scatterGoalCellPos, previousCellPos, currentCellPos);
            case GhostMode.Frightened:
                var candidatePositions = GetCandidateCellTargets(previousCellPos, currentCellPos).Select(tuple => tuple.Pos).ToList();
                return candidatePositions[Random.Range(0, candidatePositions.Count)];
            case GhostMode.RunningHome:
                return GetNextTargetCellTowardsGoal(GhostManager.GhostHomeCenter, previousCellPos, currentCellPos);
            case GhostMode.Idle:
            default:
                return currentCellPos;
        }
    }

    //Moves greedily towards the target cell based on a straight-line-distance heuristic to the goalCellPos
    protected Vector3Int GetNextTargetCellTowardsGoal(Vector3Int goalCellPos, Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        var candidateCellTargets = GetCandidateCellTargets(previousCellPos, currentCellPos);

        if (candidateCellTargets.Count > 1)
        {
            (Direction Direction, Vector3Int Pos) closestCandidate = (Direction.None, Vector3Int.zero);
            float shortestDistance = float.PositiveInfinity;

            foreach (var candidate in candidateCellTargets)
            {
                var dist = Vector3Int.Distance(candidate.Pos, goalCellPos);
                if (dist < shortestDistance
                    || (dist == shortestDistance && candidate.Direction < closestCandidate.Direction))
                {
                    closestCandidate = candidate;
                    shortestDistance = dist;
                }
            }

            return closestCandidate.Pos;
        }
        else if (candidateCellTargets.Count == 1)
        {
            return candidateCellTargets[0].Pos;
        }

        return currentCellPos;
    }

    // Assumes all surrounding cells are candidates, then prunes previous cell, wall cells, and cells that are direction-blocked for ghosts
    protected List<(Direction Direction, Vector3Int Pos)> GetCandidateCellTargets(Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        if (currentCellPos == GhostManager.GhostHomeCenter)
        {
            return new List<(Direction Direction, Vector3Int Pos)>()
            {
                (Direction.Up, currentCellPos + new Vector3Int(0, 1, 0))
            };
        }

        var result = new List<(Direction Direction, Vector3Int Pos)>()
        {
            (Direction.Left, currentCellPos + new Vector3Int(-1, 0, 0)),
            (Direction.Up, currentCellPos + new Vector3Int(0, 1, 0)),
            (Direction.Right, currentCellPos + new Vector3Int(1, 0, 0)),
            (Direction.Down, currentCellPos + new Vector3Int(0, -1, 0))
        };

        result.RemoveAll(c =>
        {
            var upBlocked = c.Direction == Direction.Up && GhostManager.Instance.UpBlockersTilemap.HasTile(currentCellPos);
            var downBlocked = c.Direction == Direction.Down && GhostManager.Instance.DownBlockersTilemap.HasTile(currentCellPos);

            return GhostManager.Instance.WallsTilemap.HasTile(c.Pos)
            || c.Pos == previousCellPos 
            || (Mode != GhostMode.RunningHome && (upBlocked || downBlocked));
        });

        return result.Count > 0 ? result : new List<(Direction, Vector3Int Pos)>() { (Direction.None, currentCellPos) };
    }

    public Vector3Int DebugGoalCell() => GetGoalCell();

    public void ResetState()
    {
        released = false;
        Mode = GhostManager.Instance.CurrentModeInQueue;
        MovementHandler.ResetState();

        AdditionalResetBehaviour();
    }

    protected abstract Vector3Int GetGoalCell();
    protected abstract void AdditionalResetBehaviour();
}
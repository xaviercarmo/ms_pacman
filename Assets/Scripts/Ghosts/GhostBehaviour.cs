using System.Collections.Generic;
using UnityEngine;

public enum GhostMode
{
    Idle,
    Chase,
    Scatter,
    Frightened
}

public abstract class GhostBehaviour
{
    //Public fields/properties
    public GhostMovement MovementHandler;

    //Protected field/properties
    protected GhostMode mode = GhostMode.Idle;

    protected Vector3Int scatterGoalCellPos;
    protected int dotsBeforeRelease = 0;
    protected float secondsBeforeRelease = 0;

    //Private fields/properties
    bool released = false;

    //Sets the ghost mode and reverses movement when previous mode was chase or scatter
    public void SetMode(GhostMode mode)
    {
        if (this.mode == GhostMode.Chase || this.mode == GhostMode.Scatter)
        {
            MovementHandler.ShouldReverseMovement = true;
        }

        this.mode = mode;
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

        switch (mode)
        {
            case GhostMode.Chase:
                return GetNextTargetCellTowardsGoal(GetGoalCell(), previousCellPos, currentCellPos);
            case GhostMode.Scatter:
                return GetNextTargetCellTowardsGoal(scatterGoalCellPos, previousCellPos, currentCellPos);
            case GhostMode.Frightened:
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

    // Assumes all surrounding cells are candidates, then prunes previous cell and wall cells
    protected List<(Direction Direction, Vector3Int Pos)> GetCandidateCellTargets(Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        var result = new List<(Direction Direction, Vector3Int Pos)>()
        {
            (Direction.Left, currentCellPos + new Vector3Int(-1, 0, 0)),
            (Direction.Up, currentCellPos + new Vector3Int(0, 1, 0)),
            (Direction.Right, currentCellPos + new Vector3Int(1, 0, 0)),
            (Direction.Down, currentCellPos + new Vector3Int(0, -1, 0))
        };

        result.RemoveAll(c =>
        {
            return GhostManager.Instance.WallsTilemap.HasTile(c.Pos)
            || c.Pos == previousCellPos
            || (c.Direction == Direction.Up && GhostManager.Instance.UpBlockersTilemap.HasTile(currentCellPos))
            || (c.Direction == Direction.Down && GhostManager.Instance.DownBlockersTilemap.HasTile(currentCellPos));
        });

        return result;
    }

    public Vector3Int DebugGoalCell() => GetGoalCell();

    public void ResetState()
    {
        released = false;
        MovementHandler.ResetState();
        AdditionalResetBehaviour();
    }

    protected abstract Vector3Int GetGoalCell();
    protected abstract void AdditionalResetBehaviour();
}
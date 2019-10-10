using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RedGhostBehaviour : GhostBehaviour
{
    public RedGhostBehaviour(GameObject player, Grid levelGrid, Tilemap wallsTilemap, Tilemap upBlockersTilemap, Tilemap downBlockersTilemap)
    {
        this.player = player;
        this.levelGrid = levelGrid;
        this.wallsTilemap = wallsTilemap;
        this.upBlockersTilemap = upBlockersTilemap;
        this.downBlockersTilemap = downBlockersTilemap;

        scatterGoalCellPos = new Vector3Int(wallsTilemap.cellBounds.xMax, wallsTilemap.cellBounds.yMax, 0);
    }

    // Uses red ghost logic to decide which cell to tween to next
    public override Vector3Int GetNextTargetCellPos(Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        switch (mode)
        {
            case GhostMode.Chase:
                return GetNextTargetCellTowardsGoal(levelGrid.WorldToCell(player.transform.position), previousCellPos, currentCellPos);
            case GhostMode.Scatter:
                return GetNextTargetCellTowardsGoal(scatterGoalCellPos, previousCellPos, currentCellPos);
            case GhostMode.Frightened:
            case GhostMode.Idle:
            default:
                return currentCellPos;
        }
    }

    // Moves greedily towards the target cell based on a straight-line-distance heuristic to the goalCellPos
    Vector3Int GetNextTargetCellTowardsGoal(Vector3Int goalCellPos, Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        var candidateCellTargets = GetCandidateCellTargets(previousCellPos, currentCellPos);

        if (candidateCellTargets.Count > 1)
        {
            (Direction Direction, Vector3Int Pos) closestCandidate = (Direction.None, Vector3Int.zero);
            float shortestDistance = -1;

            foreach (var candidate in candidateCellTargets)
            {
                var dist = Vector3Int.Distance(candidate.Pos, goalCellPos);
                if (dist < shortestDistance
                    || (dist == shortestDistance && candidate.Direction < closestCandidate.Direction)
                    || shortestDistance == -1)
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
    List<(Direction Direction, Vector3Int Pos)> GetCandidateCellTargets(Vector3Int previousCellPos, Vector3Int currentCellPos)
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
            return wallsTilemap.HasTile(c.Pos)
            || c.Pos == previousCellPos
            || (c.Direction == Direction.Up && upBlockersTilemap.HasTile(currentCellPos))
            || (c.Direction == Direction.Down && downBlockersTilemap.HasTile(currentCellPos));
        });

        return result;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RedGhostBehaviour : IGhostBehaviour
{
    public GhostMode Mode { get; set; }

    GameObject player;
    Grid levelGrid;
    Tilemap wallsTileMap;

    public RedGhostBehaviour(GameObject player, Grid levelGrid, Tilemap wallsTileMap)
    {
        this.player = player;
        this.levelGrid = levelGrid;
        this.wallsTileMap = wallsTileMap;
    }

    // Uses red ghost logic to decide which cell to tween to next
    public Vector3Int GetNextTargetCellPos(Vector3Int previousCellPos, Vector3Int currentCellPos)
    {
        var candidateCellTargets = GetCandidateCellTargets(previousCellPos, currentCellPos);
        var playerCellPos = levelGrid.WorldToCell(player.transform.position);

        if (candidateCellTargets.Count > 1)
        {
            (Direction Direction, Vector3Int Pos) closestCandidate = (Direction.None, Vector3Int.zero);
            float shortestDistance = -1;

            foreach (var candidate in candidateCellTargets)
            {
                var dist = Vector3Int.Distance(candidate.Pos, playerCellPos);
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
        else
        {
            return currentCellPos;
        }
    }

    void SeekPlayer()
    {

    }

    (Direction, bool)[] GetTileOccupancyAroundCellPos(Vector3Int cellPos)
    {
        // Returns whether surrounding cells are walls. Order: Left, Up, Right, Down
        return new (Direction, bool)[]
        {
            (Direction.Left, wallsTileMap.HasTile(cellPos + new Vector3Int(-1, 0, 0))),
            (Direction.Up, wallsTileMap.HasTile(cellPos + new Vector3Int(0, 1, 0))),
            (Direction.Right, wallsTileMap.HasTile(cellPos + new Vector3Int(1, 0, 0))),
            (Direction.Down, wallsTileMap.HasTile(cellPos + new Vector3Int(0, -1, 0)))
        };
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

        result.RemoveAll(c => wallsTileMap.HasTile(c.Pos) || c.Pos == previousCellPos);

        return result;
    }
}

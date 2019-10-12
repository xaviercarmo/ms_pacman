using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitMovement : MonoBehaviour
{
    public FruitMode Mode;

    Vector3Int initialCellPos;
    Vector3Int previousCellPos;
    Vector3Int currentCellPos;
    Vector3Int targetCellPos;

    float timeToTravelGridSize = 0.32f;

    Tweener tweener;
    Tween tween;

    void Awake()
    {
        tweener = GetComponent<Tweener>();
    }

    // Use this for initialization
    void Start()
    {
        initialCellPos = GhostManager.Instance.LevelGrid.WorldToCell(transform.position);
        currentCellPos = initialCellPos;
        targetCellPos = currentCellPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (OriginalLevelManager.Instance.GameSuspended) { return; }

        if (!tweener.TweenExists(transform, out var existingTween) || (Time.time - tween.StartTime) >= tween.Duration)
        {
            previousCellPos = currentCellPos;
            currentCellPos = targetCellPos;

            if (GhostManager.Instance.HorizontalPortalsTilemap.HasTile(currentCellPos))
            {
                if (transform.position.x < 0)
                {
                    currentCellPos = new Vector3Int(GhostManager.Instance.HorizontalPortalsTilemap.cellBounds.xMax - 1, currentCellPos.y, 0);
                    targetCellPos = new Vector3Int(GhostManager.Instance.HorizontalPortalsTilemap.cellBounds.xMax - 2, currentCellPos.y, 0);
                }
                else
                {
                    currentCellPos = new Vector3Int(GhostManager.Instance.HorizontalPortalsTilemap.cellBounds.xMin, currentCellPos.y, 0);
                    targetCellPos = new Vector3Int(GhostManager.Instance.HorizontalPortalsTilemap.cellBounds.xMin + 1, currentCellPos.y, 0);
                }

                TweenToTargetCell();
            }
            else
            {
                UpdateTargetCell();

                if (targetCellPos != currentCellPos)
                {
                    if (currentCellPos - previousCellPos != targetCellPos - currentCellPos)
                    {
                        TweenToTargetCell();
                    }
                    else
                    {
                        tween.StartPos = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(currentCellPos);
                        tween.EndPos = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(targetCellPos);
                        tween.StartTime = Time.time - (Time.time - tween.StartTime - tween.Duration);
                    }
                }
            }
        }
    }

    void TweenToTargetCell()
    {
        var currentCellCenter = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(currentCellPos);
        var targetCellCenter = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(targetCellPos);
        tween = tweener.AddTween(transform, currentCellCenter, targetCellCenter, timeToTravelGridSize, true);
    }

    void UpdateTargetCell()
    {
        switch(Mode)
        {
            case FruitMode.Random:
                var candidatePositions = GetCandidateCellTargets();
                targetCellPos = candidatePositions[Random.Range(0, candidatePositions.Count)];
                break;
            case FruitMode.Exit:
                targetCellPos = GetNextTargetCellTowardsGoal(initialCellPos);
                break;
        }
    }

    protected Vector3Int GetNextTargetCellTowardsGoal(Vector3Int goalCellPos)
    {
        var candidateCellTargets = GetCandidateCellTargets();

        if (candidateCellTargets.Count > 1)
        {
            Vector3Int closestCandidate = Vector3Int.zero;
            float shortestDistance = float.PositiveInfinity;

            foreach (var candidate in candidateCellTargets)
            {
                var dist = Vector3Int.Distance(candidate, goalCellPos);
                if (dist < shortestDistance)
                {
                    closestCandidate = candidate;
                    shortestDistance = dist;
                }
            }

            return closestCandidate;
        }
        else if (candidateCellTargets.Count == 1)
        {
            return candidateCellTargets[0];
        }

        return currentCellPos;
    }

    // Assumes all surrounding cells are candidates, then prunes previous cell and wall cells
    protected List<Vector3Int> GetCandidateCellTargets()
    {
        var result = new List<Vector3Int>()
        {
            currentCellPos + new Vector3Int(-1, 0, 0),
            currentCellPos + new Vector3Int(0, 1, 0),
            currentCellPos + new Vector3Int(1, 0, 0),
            currentCellPos + new Vector3Int(0, -1, 0)
        };

        result.RemoveAll(pos => FruitManager.Instance.WallsTilemap.HasTile(pos) || pos == previousCellPos);

        return result;
    }
}

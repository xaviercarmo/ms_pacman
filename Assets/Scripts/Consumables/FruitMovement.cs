﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitMovement : MonoBehaviour
{
    public FruitMode Mode = FruitMode.Random;

    Vector3Int exitCellPos;
    Vector3Int previousCellPos;
    Vector3Int currentCellPos;
    Vector3Int targetCellPos;

    float timeToTravelGridSize = 0.32f;

    Tweener tweener;
    CellTween tween;

    void Awake()
    {
        tweener = GetComponent<Tweener>();
        tweener.SuspendWhenGameSuspended = false;
    }

    void Start()
    {
        var initialCellPos = GhostManager.Instance.LevelGrid.WorldToCell(transform.position);
        exitCellPos = transform.position.x > 0
            ? initialCellPos - new Vector3Int(2, 0, 0)
            : initialCellPos + new Vector3Int(2, 0, 0);
        currentCellPos = initialCellPos;
        targetCellPos = initialCellPos;
    }

    void Update()
    {
        if (!tweener.TweenExists(transform, out var existingTween) || (Time.time - tween.StartTime) >= tween.Duration)
        {
            previousCellPos = currentCellPos;
            currentCellPos = targetCellPos;

            if (GhostManager.Instance.HorizontalPortalsTilemap.HasTile(currentCellPos))
            {
                if (Mode == FruitMode.Exit)
                {
                    Destroy(gameObject);
                    return;
                }

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
                        tween.StartCellPos = currentCellPos;
                        tween.EndCellPos = targetCellPos;
                        tween.UpdateWorldPositions();
                        tween.StartTime = Time.time - (Time.time - tween.StartTime - tween.Duration);
                    }
                }
            }
        }
    }

    void TweenToTargetCell()
    {
        tween = tweener.AddTween(transform, currentCellPos, targetCellPos, timeToTravelGridSize, true);
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
                targetCellPos = GetNextTargetCellTowardsGoal(exitCellPos);
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        FruitManager.Instance.ConsumeFruit(gameObject);
    }
}

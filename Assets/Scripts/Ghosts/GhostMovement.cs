using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class GhostMovement : MonoBehaviour
{
    public Grid levelGrid;
    public Tilemap wallsTilemap;
    public Tilemap blockersTilemap;
    public IGhostBehaviour behaviour;

    Vector3Int movement;
    float timeToTravelGridSize = 0.15f;

    Tweener tweener;
    new SpriteRenderer renderer;
    Animator animator;

    Vector3Int previousCellPos;
    Vector3Int currentCellPos;
    Vector3Int targetCellPos;

    void Start()
    {
        tweener = GetComponent<Tweener>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentCellPos = levelGrid.WorldToCell(transform.position);
        targetCellPos = currentCellPos;
    }

    void Update()
    {
        if (!tweener.TweenExists(transform, out var _))
        {
            previousCellPos = currentCellPos;
            currentCellPos = targetCellPos;
            UpdateTargetCellAndAnimation();

            if (targetCellPos != currentCellPos)
            {
                TweenToTargetCell();
            }
        }
    }

    void UpdateTargetCellAndAnimation()
    {
        var oldDirectionVec = (currentCellPos - previousCellPos);
        oldDirectionVec.Clamp(Vector3Int.one * -1, Vector3Int.one);

        targetCellPos = behaviour.GetNextTargetCellPos(previousCellPos, currentCellPos);

        var newDirectionVec = (targetCellPos - currentCellPos);
        newDirectionVec.Clamp(Vector3Int.one * -1, Vector3Int.one);

        if (oldDirectionVec != newDirectionVec)
        {
            if (newDirectionVec.x > 0)
            {
                animator.SetTrigger("TurnRight");
            }
            else if (newDirectionVec.x < 0)
            {
                animator.SetTrigger("TurnLeft");
            }
            else if (newDirectionVec.y > 0)
            {
                animator.SetTrigger("TurnUp");
            }
            else if (newDirectionVec.y < 0)
            {
                animator.SetTrigger("TurnDown");
            }
            else
            {
                animator.SetTrigger("StandStill");
            }
        }

    }

    void TweenToTargetCell()
    {
        var currentCellCenter = wallsTilemap.GetCellCenterWorld(currentCellPos);
        var targetCellCenter = wallsTilemap.GetCellCenterWorld(targetCellPos);
        tweener.AddTween(transform, currentCellCenter, targetCellCenter, timeToTravelGridSize);
    }
}

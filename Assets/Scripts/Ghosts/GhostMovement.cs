using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class GhostMovement : MonoBehaviour
{
    //Public fields/properties
    public Grid LevelGrid;
    public Tilemap WallsTilemap;
    public Tilemap BlockersTilemap;
    public GhostBehaviour Behaviour;

    public bool ShouldReverseMovement = false;

    //Private fields/properties
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

        currentCellPos = LevelGrid.WorldToCell(transform.position);
        targetCellPos = currentCellPos;
    }

    void Update()
    {
        if (!tweener.TweenExists(transform, out var existingTween))
        {
            previousCellPos = currentCellPos;
            currentCellPos = targetCellPos;
            UpdateTargetCellAndAnimation(ShouldReverseMovement);

            if (targetCellPos != currentCellPos)
            {
                TweenToTargetCell();
            }

            ShouldReverseMovement = false;
        }
        else if (ShouldReverseMovement)
        {
            existingTween.Reverse();
            (currentCellPos, targetCellPos) = (targetCellPos, currentCellPos);

            UpdateTargetCellAndAnimation(true);

            ShouldReverseMovement = false;
        }
    }

    void UpdateTargetCellAndAnimation(bool reverse = false)
    {
        var oldDirectionVec = currentCellPos - previousCellPos;
        oldDirectionVec.Clamp(Vector3Int.one * -1, Vector3Int.one);

        if (!reverse)
        {
            targetCellPos = Behaviour.GetNextTargetCellPos(previousCellPos, currentCellPos);
        }

        var newDirectionVec = targetCellPos - currentCellPos;
        newDirectionVec.Clamp(Vector3Int.one * -1, Vector3Int.one);

        if (oldDirectionVec != newDirectionVec || reverse)
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
        var currentCellCenter = WallsTilemap.GetCellCenterWorld(currentCellPos);
        var targetCellCenter = WallsTilemap.GetCellCenterWorld(targetCellPos);
        tweener.AddTween(transform, currentCellCenter, targetCellCenter, timeToTravelGridSize);
    }
}

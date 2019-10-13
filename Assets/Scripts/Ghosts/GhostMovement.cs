using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class GhostMovement : MonoBehaviour
{
    //Public fields/properties
    public GhostBehaviour Behaviour;

    public Vector3 InitialWorldPos;

    public Vector3Int PreviousCellPos { get; private set; }
    public Vector3Int CurrentCellPos { get; private set; }
    public Vector3Int TargetCellPos { get; private set; }

    public bool ShouldReverseMovement = false;

    //Private fields/properties
    float baseTimeToTravelGridSize = 0.16f;
    float timeToTravelGridSize = 0.16f;

    Tweener tweener;
    Tween tween;
    new SpriteRenderer renderer;
    Animator animator;

    void Awake()
    {
        tweener = GetComponent<Tweener>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        CurrentCellPos = GhostManager.Instance.LevelGrid.WorldToCell(transform.position);
        TargetCellPos = CurrentCellPos;
    }

    void Update()
    {
        if (OriginalLevelManager.Instance.GameSuspended) { UpdateAnimation(); return; }

        //Change the movement speed based on the current mode, faster when running home after being caught, slower when running away
        switch (Behaviour.Mode)
        {
            case GhostMode.Frightened:
                timeToTravelGridSize = baseTimeToTravelGridSize * 1.5f;
                break;
            case GhostMode.RunningHome:
                timeToTravelGridSize = baseTimeToTravelGridSize * 0.5f;
                break;
            default:
                timeToTravelGridSize = baseTimeToTravelGridSize;
                break;
        }

        if (!tweener.TweenExists(transform, out var existingTween) || (Time.time - tween.StartTime) >= tween.Duration)
        {
            PreviousCellPos = CurrentCellPos;
            CurrentCellPos = TargetCellPos;

            if (GhostManager.Instance.HorizontalPortalsTilemap.HasTile(CurrentCellPos))
            {
                if (transform.position.x < 0)
                {
                    CurrentCellPos = new Vector3Int(GhostManager.Instance.HorizontalPortalsTilemap.cellBounds.xMax - 1, CurrentCellPos.y, 0);
                    TargetCellPos = new Vector3Int(GhostManager.Instance.HorizontalPortalsTilemap.cellBounds.xMax - 2, CurrentCellPos.y, 0);
                }
                else
                {
                    CurrentCellPos = new Vector3Int(GhostManager.Instance.HorizontalPortalsTilemap.cellBounds.xMin, CurrentCellPos.y, 0);
                    TargetCellPos = new Vector3Int(GhostManager.Instance.HorizontalPortalsTilemap.cellBounds.xMin + 1, CurrentCellPos.y, 0);
                }

                TweenToTargetCell(1.75f);
            }
            else
            {
                if (!ShouldReverseMovement)
                {
                    TargetCellPos = Behaviour.GetNextTargetCellPos(PreviousCellPos, CurrentCellPos);
                }

                if (TargetCellPos != CurrentCellPos)
                {
                    var durationMultiplier = GhostManager.Instance.GhostSlowersTilemap.HasTile(TargetCellPos) ? 1.75f : 1f;
                    if (CurrentCellPos - PreviousCellPos != TargetCellPos - CurrentCellPos)
                    {
                        TweenToTargetCell(durationMultiplier);
                    }
                    else
                    {
                        tween.StartPos = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(CurrentCellPos);
                        tween.EndPos = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(TargetCellPos);
                        tween.StartTime = Time.time - (Time.time - tween.StartTime - tween.Duration);
                        tween.Duration = timeToTravelGridSize * durationMultiplier;
                    }
                }

                ShouldReverseMovement = false;
            }
        }
        else if (ShouldReverseMovement)
        {
            existingTween.Reverse();
            (CurrentCellPos, TargetCellPos) = (TargetCellPos, CurrentCellPos);

            ShouldReverseMovement = false;
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        var directionVec = TargetCellPos - CurrentCellPos;

        if (Behaviour.Mode == GhostMode.Frightened)
        {
            animator.SetTrigger("RunAway");
            animator.speed = 0.5f;
        }
        else if (Behaviour.Mode == GhostMode.RunningHome)
        {
            animator.SetTrigger("RunHome");
            animator.speed = 1f;
        }
        else
        {
            animator.speed = 1;
            if (directionVec.x > 0)
            {
                animator.SetTrigger("TurnRight");
            }
            else if (directionVec.x < 0)
            {
                animator.SetTrigger("TurnLeft");
            }
            else if (directionVec.y > 0)
            {
                animator.SetTrigger("TurnUp");
            }
            else if (directionVec.y < 0)
            {
                animator.SetTrigger("TurnDown");
            }
            else
            {
                animator.SetTrigger("StandStill");
            }
        }
    }

    void TweenToTargetCell(float durationMultiplier = 1)
    {
        var currentCellCenter = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(CurrentCellPos);
        var targetCellCenter = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(TargetCellPos);
        tween = tweener.AddTween(transform, currentCellCenter, targetCellCenter, timeToTravelGridSize * durationMultiplier, true);
    }

    public void ResetState()
    {
        tween = null;
        tweener.FlushTweens();

        transform.position = InitialWorldPos;
        CurrentCellPos = GhostManager.Instance.LevelGrid.WorldToCell(InitialWorldPos);
        TargetCellPos = CurrentCellPos;

        animator.SetTrigger("StandStill");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (Behaviour.Mode != GhostMode.Frightened && Behaviour.Mode != GhostMode.RunningHome)
        {
            OriginalLevelManager.Instance.ResetLevel();
        }
        else if (Behaviour.Mode == GhostMode.Frightened)
        {
            GhostManager.Instance.ConsecutiveGhostsEaten++;
            AudioManager.Instance.GhostEatenAudioSource.Play();
            PlayerManager.Instance.Points += 200 * GhostManager.Instance.ConsecutiveGhostsEaten;
            Behaviour.SetMode(GhostMode.RunningHome);
        }
    }
}

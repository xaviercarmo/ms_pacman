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
        if (OriginalLevelManager.Instance.GameResetting) { return; }

        if (!tweener.TweenExists(transform, out var existingTween) || (Time.time - tween.StartTime) >= tween.Duration)
        {
            PreviousCellPos = CurrentCellPos;
            CurrentCellPos = TargetCellPos;

            UpdateTargetCellAndAnimation(ShouldReverseMovement);

            if (GhostManager.Instance.LevelGrid.WorldToCell(transform.position) == GhostManager.Instance.LevelGrid.WorldToCell(PlayerManager.Instance.gameObject.transform.position))
            {
                OriginalLevelManager.Instance.ResetLevel();
                return;
            }

            if (TargetCellPos != CurrentCellPos)
            {
                if (CurrentCellPos - PreviousCellPos != TargetCellPos - CurrentCellPos)
                {
                    TweenToTargetCell();
                }
                else
                {
                    tween.StartPos = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(CurrentCellPos);
                    tween.EndPos = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(TargetCellPos);
                    tween.StartTime = Time.time - (Time.time - tween.StartTime - tween.Duration);
                }
            }

            ShouldReverseMovement = false;
        }
        else if (ShouldReverseMovement)
        {
            existingTween.Reverse();
            (CurrentCellPos, TargetCellPos) = (TargetCellPos, CurrentCellPos);

            UpdateTargetCellAndAnimation(true);

            ShouldReverseMovement = false;
        }

        //if (Behaviour is PinkGhostBehaviour || Behaviour is BlueGhostBehaviour)
        //{
        //    var currentCellCenter = WallsTilemap.GetCellCenterWorld(CurrentCellPos);
        //    var goalCellCenter = WallsTilemap.GetCellCenterWorld(Behaviour.DebugGoalCell());
        //    Debug.DrawLine(currentCellCenter, goalCellCenter, Color.red, 0f, false);
        //}
    }

    void UpdateTargetCellAndAnimation(bool reverse = false)
    {
        var oldDirectionVec = CurrentCellPos - PreviousCellPos;
        oldDirectionVec.Clamp(Vector3Int.one * -1, Vector3Int.one);

        if (!reverse)
        {
            TargetCellPos = Behaviour.GetNextTargetCellPos(PreviousCellPos, CurrentCellPos);
        }

        var newDirectionVec = TargetCellPos - CurrentCellPos;
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
        var currentCellCenter = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(CurrentCellPos);
        var targetCellCenter = GhostManager.Instance.WallsTilemap.GetCellCenterWorld(TargetCellPos);
        tween = tweener.AddTween(transform, currentCellCenter, targetCellCenter, timeToTravelGridSize, true);
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
}

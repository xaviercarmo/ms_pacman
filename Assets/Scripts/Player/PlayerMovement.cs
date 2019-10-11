using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    Up,
    Left,
    Down,
    Right,
    None
}

public class PlayerMovement : MonoBehaviour
{
    public Grid levelGrid;
    public Tilemap wallsTilemap;
    public Tilemap consumablesTilemap;
    public Tilemap horizontalPortalsTilemap;

    public Vector3Int PreviousCellPos { get; private set; }
    public Vector3Int CurrentCellPos { get; private set; }
    public Vector3Int TargetCellPos { get; private set; }

    Vector3Int movement;
    float timeToTravelGridSize = 0.15f;

    Tween tween;

    KeyCode lastMovementKeyApplied;

    bool flipX = false;
    bool flipY = false;
    Quaternion rotation = Quaternion.identity;

    void Start()
    {
        CurrentCellPos = levelGrid.WorldToCell(transform.position);
        TargetCellPos = CurrentCellPos + new Vector3Int(1, 0, 0);

        TweenToTargetCell();
    }

    void Update()
    {
        if (OriginalLevelManager.Instance.GameResetting) { return; }

        if (!PlayerManager.Tweener.TweenExists(transform, out var existingTween) || (Time.time - tween.StartTime) >= tween.Duration)
        {
            PreviousCellPos = CurrentCellPos;
            CurrentCellPos = TargetCellPos;

            if (horizontalPortalsTilemap.HasTile(CurrentCellPos))
            {
                if (transform.position.x < 0)
                {
                    CurrentCellPos = new Vector3Int(horizontalPortalsTilemap.cellBounds.xMax - 1, CurrentCellPos.y, 0);
                    TargetCellPos = new Vector3Int(horizontalPortalsTilemap.cellBounds.xMax - 2, CurrentCellPos.y, 0);
                }
                else
                {
                    CurrentCellPos = new Vector3Int(horizontalPortalsTilemap.cellBounds.xMin, CurrentCellPos.y, 0);
                    TargetCellPos = new Vector3Int(horizontalPortalsTilemap.cellBounds.xMin + 1, CurrentCellPos.y, 0);
                }

                TweenToTargetCell();
            }
            else
            {
                var updatedTargetCellPos = GetUpdatedTargetCellAndSprite();

                if (consumablesTilemap.HasTile(CurrentCellPos))
                {
                    consumablesTilemap.SetTile(CurrentCellPos, null);
                    PlayerManager.DotsEaten++;
                }

                if (updatedTargetCellPos != CurrentCellPos)
                {
                    TargetCellPos = updatedTargetCellPos;

                    if (CurrentCellPos - PreviousCellPos != TargetCellPos - CurrentCellPos)
                    {
                        TweenToTargetCell();
                    }
                    else
                    {
                        tween.StartPos = wallsTilemap.GetCellCenterWorld(CurrentCellPos);
                        tween.EndPos = wallsTilemap.GetCellCenterWorld(TargetCellPos);
                        tween.StartTime = Time.time - (Time.time - tween.StartTime - tween.Duration);
                    }
                }
            }
        }
        else if (IsMovementKeyRetrograde())
        {
            lastMovementKeyApplied = PlayerInputManager.LastMovementKeyPressed;

            existingTween.Reverse();
            (CurrentCellPos, TargetCellPos) = (TargetCellPos, CurrentCellPos);

            ReverseFlipAndRotation();
        }

        //var currentCellCenter = wallsTilemap.GetCellCenterWorld(CurrentCellPos);
        //var targetCellCenter = wallsTilemap.GetCellCenterWorld(TargetCellPos);
        //Debug.DrawLine(currentCellCenter, targetCellCenter, Color.red, 0f, false);
        //Debug.DrawLine(tween.StartPos, tween.EndPos, Color.red, 0f, false);
    }

    //Animtor components set rotation every frame, so to apply custom-rotation this late-update is required
    void LateUpdate()
    {
        ApplyFlipAndRotation();
    }

    public void ResetState()
    {
        transform.position = PlayerManager.HomeWorldPos;

        CurrentCellPos = levelGrid.WorldToCell(PlayerManager.HomeWorldPos);
        TargetCellPos = CurrentCellPos + new Vector3Int(1, 0, 0);
        flipX = false;
        flipY = false;
        rotation = Quaternion.identity;

        PlayerManager.Tweener.FlushTweens();
    }

    public void ResumeAfterReset()
    {
        TweenToTargetCell();
    }

    void TweenToTargetCell()
    {
        var currentCellCenter = wallsTilemap.GetCellCenterWorld(CurrentCellPos);
        var targetCellCenter = wallsTilemap.GetCellCenterWorld(TargetCellPos);
        tween = PlayerManager.Tweener.AddTween(transform, currentCellCenter, targetCellCenter, timeToTravelGridSize, true);
    }

    void ApplyFlipAndRotation()
    {
        PlayerManager.Renderer.flipX = flipX;
        PlayerManager.Renderer.flipY = flipY;
        transform.rotation = rotation;
    }

    void ReverseFlipAndRotation()
    {
        if (lastMovementKeyApplied == PlayerInputManager.Controls.Down || lastMovementKeyApplied == PlayerInputManager.Controls.Up)
        {
            flipY = !flipY;
            rotation.z *= -1;
        }
        else if (lastMovementKeyApplied == PlayerInputManager.Controls.Left || lastMovementKeyApplied == PlayerInputManager.Controls.Right)
        {
            flipX = !flipX;
        }
    }

    Vector3Int GetUpdatedTargetCellAndSprite()
    {
        var result = CurrentCellPos;

        if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Up)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipY = PlayerManager.Renderer.flipX;
                flipX = false;

                rotation = Quaternion.Euler(0, 0, 90);
            }

            result = new Vector3Int(CurrentCellPos.x, CurrentCellPos.y + (int)levelGrid.cellSize.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Left)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipX = true;
                flipY = false;

                rotation = Quaternion.identity;
            }

            result = new Vector3Int(CurrentCellPos.x - (int)levelGrid.cellSize.x, CurrentCellPos.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Down)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipY = PlayerManager.Renderer.flipX;
                flipX = false;

                rotation = Quaternion.Euler(0, 0, -90);
            }

            result = new Vector3Int(CurrentCellPos.x, CurrentCellPos.y - (int)levelGrid.cellSize.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Right)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipX = false;
                flipY = false;

                rotation = Quaternion.identity;
            }

            result = new Vector3Int(CurrentCellPos.x + (int)levelGrid.cellSize.x, CurrentCellPos.y, 0);
        }

        //if the target cell is a wall
        if (wallsTilemap.HasTile(result))
        {
            flipY = PlayerManager.Renderer.flipY;
            flipX = PlayerManager.Renderer.flipX;
            rotation = transform.rotation;

            //then try to continue the current motion
            result = CurrentCellPos + (CurrentCellPos - PreviousCellPos);

            //if there is a wall straight ahead
            if (wallsTilemap.HasTile(result))
            {
                //then stand still
                result = CurrentCellPos;
            }
        }
        else
        {
            lastMovementKeyApplied = PlayerInputManager.LastMovementKeyPressed;
        }

        return result;
    }

    bool IsMovementKeyRetrograde()
    {
        return (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Up && lastMovementKeyApplied == PlayerInputManager.Controls.Down)
            || (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Down && lastMovementKeyApplied == PlayerInputManager.Controls.Up)
            || (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Left && lastMovementKeyApplied == PlayerInputManager.Controls.Right)
            || (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Right && lastMovementKeyApplied == PlayerInputManager.Controls.Left);
    }
}

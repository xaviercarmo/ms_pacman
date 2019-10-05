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

    Vector3Int movement;
    float timeToTravelGridSize = 0.15f;

    Tweener tweener;
    new SpriteRenderer renderer;

    KeyCode lastMovementKeyApplied;

    Vector3Int previousCellPos;
    Vector3Int currentCellPos;
    Vector3Int targetCellPos;

    bool flipX = false;
    bool flipY = false;
    Quaternion rotation = Quaternion.identity;

    void Start()
    {
        tweener = GetComponent<Tweener>();
        renderer = GetComponent<SpriteRenderer>();

        currentCellPos = levelGrid.WorldToCell(transform.position);
        targetCellPos = currentCellPos + new Vector3Int(1, 0, 0);

        TweenToTargetCell();
    }

    void Update()
    {
        if (!tweener.TweenExists(transform, out var existingTween))
        {
            previousCellPos = currentCellPos;
            currentCellPos = targetCellPos;
            var updatedTargetCellPos = GetUpdatedTargetCellAndSprite();

            if (updatedTargetCellPos != currentCellPos)
            {
                targetCellPos = updatedTargetCellPos;
                TweenToTargetCell();
            }
        }
        else if (IsMovementKeyRetrograde())
        {
            lastMovementKeyApplied = PlayerInputManager.LastMovementKeyPressed;

            existingTween.Reverse();
            (currentCellPos, targetCellPos) = (targetCellPos, currentCellPos);

            ReverseFlipAndRotation();
            ApplyFlipAndRotation();
        }

        var currentCellCenter = wallsTilemap.GetCellCenterWorld(currentCellPos);
        var targetCellCenter = wallsTilemap.GetCellCenterWorld(targetCellPos);
        Debug.DrawLine(currentCellCenter, targetCellCenter, Color.red, 0f, false);
    }

    void TweenToTargetCell()
    {
        ApplyFlipAndRotation();

        var currentCellCenter = wallsTilemap.GetCellCenterWorld(currentCellPos);
        var targetCellCenter = wallsTilemap.GetCellCenterWorld(targetCellPos);
        tweener.AddTween(transform, currentCellCenter, targetCellCenter, timeToTravelGridSize);
    }

    void ApplyFlipAndRotation()
    {
        renderer.flipX = flipX;
        renderer.flipY = flipY;
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
        var result = targetCellPos;

        if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Up)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipY = renderer.flipX;
                flipX = false;

                rotation = Quaternion.Euler(0, 0, 90);
            }

            result = new Vector3Int(targetCellPos.x, targetCellPos.y + (int)levelGrid.cellSize.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Left)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipX = true;
                flipY = false;

                rotation = Quaternion.identity;
            }

            result = new Vector3Int(targetCellPos.x - (int)levelGrid.cellSize.x, targetCellPos.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Down)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipY = renderer.flipX;
                flipX = false;

                rotation = Quaternion.Euler(0, 0, -90);
            }

            result = new Vector3Int(targetCellPos.x, targetCellPos.y - (int)levelGrid.cellSize.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Right)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipX = false;
                flipY = false;

                rotation = Quaternion.identity;
            }

            result = new Vector3Int(targetCellPos.x + (int)levelGrid.cellSize.x, targetCellPos.y, 0);
        }

        if (wallsTilemap.HasTile(result))
        {
            flipY = renderer.flipY;
            flipX = renderer.flipX;
            rotation = transform.rotation;

            result = targetCellPos + (targetCellPos - previousCellPos);

            if (wallsTilemap.HasTile(result))
            {
                result = currentCellPos;
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

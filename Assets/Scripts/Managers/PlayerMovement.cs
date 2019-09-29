using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Grid levelGrid;
    public Tilemap wallsTilemap;

    Vector3Int movement;
    float movementDistance = 1f;

    Tweener tweener;
    new SpriteRenderer renderer;

    KeyCode lastMovementKeyApplied;

    //Vector3 currentCellCenter;
    Vector3Int currentCellPos;
    //Vector3 targetCellCenter;
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
        //var updatedTargetCell = GetUpdatedTargetCellAndSprite();

        if (!tweener.TweenExists(transform, out var _))
        {
            var updatedTargetCellPos = GetUpdatedTargetCellAndSprite(targetCellPos);

            currentCellPos = targetCellPos;
            targetCellPos = updatedTargetCellPos;

            if (!wallsTilemap.HasTile(targetCellPos))
            {
                renderer.flipX = flipX;
                renderer.flipY = flipY;
                transform.rotation = rotation;

                TweenToTargetCell();
            }
            else
            {
                targetCellPos = currentCellPos;
            }
        }

        var currentCellCenter = wallsTilemap.GetCellCenterWorld(currentCellPos);
        var targetCellCenter = wallsTilemap.GetCellCenterWorld(targetCellPos);
        Debug.DrawLine(currentCellCenter, targetCellCenter, Color.red, 0f, false);
    }

    void TweenToTargetCell()
    {
        var currentCellCenter = wallsTilemap.GetCellCenterWorld(currentCellPos);
        var targetCellCenter = wallsTilemap.GetCellCenterWorld(targetCellPos);
        tweener.AddTween(transform, currentCellCenter, targetCellCenter, 0.25f);
    }

    Vector3Int GetUpdatedTargetCellAndSprite(Vector3Int oldTargetCellPos)
    {
        var result = oldTargetCellPos;

        if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Up)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipY = renderer.flipX;
                flipX = false;

                rotation = Quaternion.Euler(0, 0, 90);
            }

            result = new Vector3Int(oldTargetCellPos.x, oldTargetCellPos.y + (int)levelGrid.cellSize.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Left)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipX = true;
                flipY = false;

                rotation = Quaternion.identity;
            }

            result = new Vector3Int(oldTargetCellPos.x - (int)levelGrid.cellSize.x, oldTargetCellPos.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Down)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipY = renderer.flipX;
                flipX = false;

                rotation = Quaternion.Euler(0, 0, -90);
            }

            result = new Vector3Int(oldTargetCellPos.x, oldTargetCellPos.y - (int)levelGrid.cellSize.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Right)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipX = false;
                flipY = false;

                rotation = Quaternion.identity;
            }

            result = new Vector3Int(oldTargetCellPos.x + (int)levelGrid.cellSize.x, oldTargetCellPos.y, 0);
        }

        if (wallsTilemap.HasTile(result))
        {
            flipY = renderer.flipY;
            flipX = renderer.flipX;
            rotation = transform.rotation;

            result = oldTargetCellPos + (oldTargetCellPos - currentCellPos);
        }
        else
        {
            lastMovementKeyApplied = PlayerInputManager.LastMovementKeyPressed;
        }


        return result;
    }
}

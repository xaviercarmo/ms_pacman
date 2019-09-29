using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 movement;
    float movementDistance = 1f;
    Tweener tweener;
    new SpriteRenderer renderer;
    KeyCode lastKeyConsumed;

    void Start()
    {
        tweener = GetComponent<Tweener>();
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateMovementAndSprite();
        tweener.AddTween(transform, transform.position, transform.position + movement, 0.5f);
    }

    void UpdateMovementAndSprite()
    {
        var lastMovementKeyPressed = PlayerInputManager.ConsumeLastMovementKey();

        if (lastKeyConsumed == lastMovementKeyPressed) return;
        else lastKeyConsumed = lastMovementKeyPressed;

        if (lastMovementKeyPressed == PlayerInputManager.Controls.Up)
        {
            movement.x = 0;
            movement.y = movementDistance;
            renderer.flipY = renderer.flipX;
            renderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (lastMovementKeyPressed == PlayerInputManager.Controls.Left)
        {
            movement.y = 0;
            movement.x = -movementDistance;
            renderer.flipX = true;
            renderer.flipY = false;
            transform.rotation = Quaternion.identity;
        }
        else if (lastMovementKeyPressed == PlayerInputManager.Controls.Down)
        {
            movement.x = 0;
            movement.y = -movementDistance;
            renderer.flipY = renderer.flipX;
            renderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (lastMovementKeyPressed == PlayerInputManager.Controls.Right)
        {
            movement.y = 0;
            movement.x = movementDistance;
            renderer.flipX = false;
            renderer.flipY = false;
            transform.rotation = Quaternion.identity;
        }
    }
}

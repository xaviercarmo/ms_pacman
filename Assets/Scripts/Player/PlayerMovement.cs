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
    public Vector3Int PreviousCellPos { get; private set; }
    public Vector3Int CurrentCellPos { get; private set; }
    public Vector3Int TargetCellPos { get; private set; }

    Vector3Int movement;
    float timeToTravelGridSize = 0.15f;

    CellTween tween;

    KeyCode lastMovementKeyApplied;

    bool flipX = false;
    bool flipY = false;
    Quaternion rotation = Quaternion.identity;

    void Awake()
    {
    }

    void Start()
    {
        CurrentCellPos = PlayerManager.Instance.LevelGrid.WorldToCell(transform.position);
        TargetCellPos = CurrentCellPos + new Vector3Int(1, 0, 0);
        TweenToTargetCell();
    }

    void Update()
    {
        if (LevelManager.Instance.GameSuspended) { return; }

        if (!PlayerManager.Instance.Tweener.TweenExists(transform, out var existingTween) || (Time.time - tween.StartTime) >= tween.Duration)
        {
            PreviousCellPos = CurrentCellPos;
            CurrentCellPos = TargetCellPos;

            if (PlayerManager.Instance.HorizontalPortalsTilemap.HasTile(CurrentCellPos))
            {
                if (transform.position.x < 0)
                {
                    CurrentCellPos = new Vector3Int(PlayerManager.Instance.HorizontalPortalsTilemap.cellBounds.xMax - 1, CurrentCellPos.y, 0);
                    TargetCellPos = new Vector3Int(PlayerManager.Instance.HorizontalPortalsTilemap.cellBounds.xMax - 2, CurrentCellPos.y, 0);
                }
                else
                {
                    CurrentCellPos = new Vector3Int(PlayerManager.Instance.HorizontalPortalsTilemap.cellBounds.xMin, CurrentCellPos.y, 0);
                    TargetCellPos = new Vector3Int(PlayerManager.Instance.HorizontalPortalsTilemap.cellBounds.xMin + 1, CurrentCellPos.y, 0);
                }

                TweenToTargetCell();
            }
            else
            {
                var updatedTargetCellPos = GetUpdatedTargetCellAndSprite();

                var playerAudioSource = AudioManager.Instance.PlayerAudioSource;
                if (PlayerManager.Instance.DotsTilemap.HasTile(CurrentCellPos))
                {
                    PlayerManager.Instance.DotsTilemap.SetTile(CurrentCellPos, null);
                    PlayerManager.Instance.Points += 10;
                    PlayerManager.Instance.DotsEaten++;

                    if (!playerAudioSource.isPlaying)
                    {
                        playerAudioSource.clip = AudioManager.Instance.PlayerEatDotClip;
                        playerAudioSource.loop = true;
                        playerAudioSource.Play();
                    }
                }
                else
                {
                    playerAudioSource.loop = false;
                }

                if (updatedTargetCellPos != CurrentCellPos)
                {
                    TargetCellPos = updatedTargetCellPos;

                    if (CurrentCellPos - PreviousCellPos != TargetCellPos - CurrentCellPos)
                    {
                        TweenToTargetCell();
                        ApplyFlipAndRotation();
                    }
                    else
                    {
                        tween.StartCellPos = CurrentCellPos;
                        tween.EndCellPos = TargetCellPos;
                        tween.UpdateWorldPositions();
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
            ApplyFlipAndRotation();
        }
    }

    public void ResetState()
    {
        if (PlayerManager.Instance.Lives > 0)
        {
            Invoke("ResetStateDelayed", 1.5f);
        }
    }

    void ResetStateDelayed()
    {
        PlayerManager.Instance.Animator.SetTrigger("Respawned");

        CurrentCellPos = PlayerManager.Instance.LevelGrid.WorldToCell(PlayerManager.Instance.HomeWorldPos);
        TargetCellPos = CurrentCellPos + new Vector3Int(1, 0, 0);

        lastMovementKeyApplied = PlayerInputManager.LastMovementKeyPressed;
        flipX = false;
        flipY = false;
        rotation = Quaternion.identity;
        ApplyFlipAndRotation();

        transform.position = PlayerManager.Instance.HomeWorldPos;

        PlayerManager.Instance.Tweener.FlushTweens();
    }

    public void ResumeAfterReset()
    {
        TweenToTargetCell();
    }

    void TweenToTargetCell()
    {
        tween = PlayerManager.Instance.Tweener.AddTween(transform, CurrentCellPos, TargetCellPos, timeToTravelGridSize, true);
    }

    void ApplyFlipAndRotation()
    {
        PlayerManager.Instance.Renderer.flipX = flipX;
        PlayerManager.Instance.Renderer.flipY = flipY;
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
        var cellSizeX = (int)PlayerManager.Instance.LevelGrid.cellSize.x;
        var cellSizeY = (int)PlayerManager.Instance.LevelGrid.cellSize.y;

        if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Up)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipY = PlayerManager.Instance.Renderer.flipX;
                flipX = false;

                rotation = Quaternion.Euler(0, 0, 90);
            }

            result = new Vector3Int(CurrentCellPos.x, CurrentCellPos.y + cellSizeY, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Left)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipX = true;
                flipY = false;

                rotation = Quaternion.identity;
            }

            result = new Vector3Int(CurrentCellPos.x - cellSizeX, CurrentCellPos.y, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Down)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipY = PlayerManager.Instance.Renderer.flipX;
                flipX = false;

                rotation = Quaternion.Euler(0, 0, -90);
            }

            result = new Vector3Int(CurrentCellPos.x, CurrentCellPos.y - cellSizeY, 0);
        }
        else if (PlayerInputManager.LastMovementKeyPressed == PlayerInputManager.Controls.Right)
        {
            if (lastMovementKeyApplied != PlayerInputManager.LastMovementKeyPressed)
            {
                flipX = false;
                flipY = false;

                rotation = Quaternion.identity;
            }

            result = new Vector3Int(CurrentCellPos.x + cellSizeX, CurrentCellPos.y, 0);
        }

        //if the target cell is a wall
        var isTargetDownwards = result.y < CurrentCellPos.y;
        if (PlayerManager.Instance.WallsTilemap.HasTile(result) || (isTargetDownwards && PlayerManager.Instance.DownBlockersTilemap.HasTile(result)))
        {
            flipY = PlayerManager.Instance.Renderer.flipY;
            flipX = PlayerManager.Instance.Renderer.flipX;
            rotation = transform.rotation;

            //then try to continue the current motion
            result = CurrentCellPos + (CurrentCellPos - PreviousCellPos);
            isTargetDownwards = result.y < CurrentCellPos.y;

            //if there is a wall straight ahead or downwards motion is blocked
            if (PlayerManager.Instance.WallsTilemap.HasTile(result) || (isTargetDownwards && PlayerManager.Instance.DownBlockersTilemap.HasTile(result)))
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

using UnityEngine;
using System.Collections;
using System;

public class LevelManagerNew : LevelManager
{
    public Animator SpriteRowAnimator;

    protected override void EndConditions()
    {
        if (!GameSuspended)
        {
            if (PlayerManager.Instance.DotsEaten == 0 && !ScrollingMode) //change this to 152
            {
                StatusText.text = "Run!";
                canPause = false;
                GameSuspended = true;
                ScrollingMode = true;

                //GameManager.Instance.BackgroundMusic.Stop(); //commented while playing around
                AudioManager.Instance.PauseAllSources();
                AudioManager.Instance.RumbleAudioSource.Play();

                GhostManager.Instance.FrightenIndefinitely();

                SpriteRowAnimator.SetTrigger("Descend");

                Invoke("BeginScrollMode", 4f);
            }
            else if ((!ScrollingMode && PlayerManager.Instance.Lives <= 0) || (ScrollingMode && PlayerManager.Instance.Health <= 0))
            {
                StatusText.text = "Game  Over...";
                AudioManager.Instance.StopAllSources();
                PlayerManager.Instance.PlayerDieBehaviour();
                GameSuspended = true;
                canPause = false;
            }
        }
    }

    void BeginScrollMode()
    {
        canPause = true;
        GameSuspended = false;

        GridManager.Instance.DeleteBottomRow();

        Array.ForEach(PlayerManager.Instance.LifeImages, img => img.enabled = false);

        AudioManager.Instance.RumbleAudioSource.Stop();
        //GameManager.Instance.BackgroundMusic.Play(); //commented while playing around
        AudioManager.Instance.ResumeAllSources();
    }
}

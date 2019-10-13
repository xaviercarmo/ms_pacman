using UnityEngine;
using System.Collections;

public class LevelManagerOriginal : LevelManager
{
    protected override void EndConditions()
    {
        if (PlayerManager.Instance.DotsEaten == 152)
        {
            StatusText.text = "You  Won!";
            AudioManager.Instance.StopAllSources();
            GameSuspended = true;
        }
        else if (PlayerManager.Instance.Lives <= 0)
        {
            StatusText.text = "Game  Over...";
            AudioManager.Instance.StopAllSources();
            GameSuspended = true;
        }
    }

    protected override void AdditionalUpdate()
    {
    }
}

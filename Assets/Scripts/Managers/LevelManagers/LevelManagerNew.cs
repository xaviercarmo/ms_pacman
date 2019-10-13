using UnityEngine;
using System.Collections;

public class LevelManagerNew : LevelManager
{
    protected override void EndConditions()
    {
        if (PlayerManager.Instance.Lives <= 0)
        {
            StatusText.text = "Game  Over...";
            AudioManager.Instance.StopAllSources();
            GameSuspended = true;
        }
    }
}

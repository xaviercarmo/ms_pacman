using UnityEngine;
using System.Collections;

public class LevelManagerNew : LevelManager
{
    public GameObject MobileLevel;
    public Vector3 translationAmount;

    float baseScrollSpeed = 1f;
    float scrollSpeedMultiplier = 1;

    protected override void EndConditions()
    {
        if (PlayerManager.Instance.Lives <= 0)
        {
            StatusText.text = "Game  Over...";
            AudioManager.Instance.StopAllSources();
            GameSuspended = true;
        }
    }

    protected override void AdditionalUpdate()
    {
        if (gameStarted && !GameSuspended)
        {
            translationAmount = new Vector3(0, baseScrollSpeed * scrollSpeedMultiplier * Time.deltaTime, 0);
            MobileLevel.transform.Translate(translationAmount);
        }
    }
}

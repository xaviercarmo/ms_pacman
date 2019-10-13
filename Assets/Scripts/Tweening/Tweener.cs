using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tweener : MonoBehaviour
{
    public bool SuspendWhenGameSuspended = true;

    List<Tween> activeTweens;

    private void Awake()
    {
        activeTweens = new List<Tween>();
    }

    //Late update used here to allow movement handlers to explicitly handle tween-completion which allows for smoother movement
    void LateUpdate()
    {
        if (SuspendWhenGameSuspended && LevelManager.Instance.GameSuspended)
        {
            activeTweens.ForEach(tween => tween.StartTime += Time.deltaTime);
            return;
        }

        for (var i = activeTweens.Count - 1; i >= 0; i--)
        {
            var tween = activeTweens[i];
            float timeFraction = (Time.time - tween.StartTime) / tween.Duration;

            if (timeFraction < 1)
            {
                tween.Target.position = Vector3.Lerp(tween.StartPos, tween.EndPos, timeFraction);
            }
            else
            {
                tween.Target.position = tween.EndPos;
                activeTweens.RemoveAt(i);
            }
        }
    }

    public void AddTween(Tween tween, bool replaceIfTransformExists = false)
    {
        if (!TweenExists(tween.Target, out var existingTween))
        {
            activeTweens.Add(tween);
        }
        else if (replaceIfTransformExists)
        {
            activeTweens.Remove(existingTween);
            activeTweens.Add(tween);
        }
    }

    public Tween AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration, bool replaceIfTransformExists = false)
    {
        var tween = new Tween(targetObject, startPos, endPos, Time.time, duration);
        AddTween(tween, replaceIfTransformExists);
        return tween;
    }

    public bool TweenExists(Transform target, out Tween existingTween)
    {
        existingTween = activeTweens.FirstOrDefault(tween => tween.Target == target) ?? default;
        return existingTween != default(Tween);
    }
    
    public void FlushTweens()
    {
        activeTweens = new List<Tween>();
    }
}

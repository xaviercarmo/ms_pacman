using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tweener : MonoBehaviour
{
    List<Tween> activeTweens;

    void Start()
    {
        activeTweens = new List<Tween>();
    }


    void Update()
    {
        for (var i = activeTweens.Count - 1; i >= 0; i--)
        {
            var tween = activeTweens[i];
            if (Vector3.Distance(tween.Target.position, tween.EndPos) > 0.1f)
            {
                float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
                tween.Target.position = Vector3.Lerp(tween.StartPos, tween.EndPos, timeFraction);
            }
            else
            {
                tween.Target.position = tween.EndPos;
                activeTweens.RemoveAt(i);
            }
        }
    }

    public void AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration, bool replaceIfExists = true)
    {
        if (!TweenExists(targetObject, out var existingTween))
        {
            activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration));
        }
        else if (replaceIfExists)
        {
            activeTweens.Remove(existingTween);
            activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration));
        }
    }

    public bool TweenExists(Transform target, out Tween existingTween)
    {
        existingTween = activeTweens.FirstOrDefault(tween => tween.Target == target);
        return existingTween != default(Tween);
    }
}

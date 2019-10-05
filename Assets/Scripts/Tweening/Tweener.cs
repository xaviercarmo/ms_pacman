using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tweener : MonoBehaviour
{
    List<Tween> activeTweens;

    private void Awake()
    {
        activeTweens = new List<Tween>();
    }

    void Start()
    {
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

    public void AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration, bool replaceIfTransformExists = false)
    {
        AddTween(new Tween(targetObject, startPos, endPos, Time.time, duration), replaceIfTransformExists);
    }

    public bool TweenExists(Transform target, out Tween existingTween)
    {
        existingTween = activeTweens?.FirstOrDefault(tween => tween.Target == target) ?? default;
        return existingTween != default(Tween);
    }
}

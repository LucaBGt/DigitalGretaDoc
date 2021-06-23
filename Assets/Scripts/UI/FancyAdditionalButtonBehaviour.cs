using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class FancyAdditionalButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    float current = 0;

    [SerializeField] FancyButtonElement[] fancyButtonElements;
    [SerializeField] AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField] float animationDuration = 0.2f;


    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateRoutine(AnimationDirection.Up));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateRoutine(AnimationDirection.Down));
    }

    private IEnumerator AnimateRoutine(AnimationDirection direction)
    {
        while (direction == AnimationDirection.Up ? (current < 1f) : (current > 0f))
        {
            current += direction.ToFloat() * (Time.deltaTime / animationDuration);
            AnimateAllElements(current);
            yield return null;
        }
    }

    private void AnimateAllElements(float current)
    {
        if (fancyButtonElements == null || fancyButtonElements.Length == 0)
            return;

        float curveValue = animationCurve.Evaluate(current);

        foreach (FancyButtonElement element in fancyButtonElements)
        {
            element.SetScale(curveValue);
        }
    }
}

[System.Serializable]
public class FancyButtonElement
{
    public Graphic Graphic;
    public Vector3 scaleDefault = Vector3.one;
    public Vector3 scaleHover = Vector3.one * 1.1f;

    public void SetScale(float lerpValue)
    {
        Graphic.transform.localScale = Vector3.Lerp(scaleDefault, scaleHover, lerpValue);
    }
}

public enum AnimationDirection
{
    Up,
    Down,
}

public static class AnimationDirectionExtention
{
    public static float ToFloat(this AnimationDirection direction)
    {
        return direction == AnimationDirection.Up ? 1f : -1f;
    }
}
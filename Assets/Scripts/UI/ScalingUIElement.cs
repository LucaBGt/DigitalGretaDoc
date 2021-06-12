using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalingUIElement : MonoBehaviour
{
    float current = 0;
    AnimationCurve easeInAndOut = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] Transform scaling;
    [SerializeField] Graphic fading;

    public void SetActiveTransition(bool active)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionRoutine(active));
    }

    private IEnumerator TransitionRoutine(bool active)
    {
        SetActiveAll(true);

        int direction = active ? 1 : -1;
        while (direction > 0 ? current < 1 : current > 0)
        {
            current += direction * Time.deltaTime;
            UpdateVisuals();
            yield return null;
        }

        if (!active)
            SetActiveAll(false);

        yield return null;
    }

    private void SetActiveAll(bool active)
    {
        scaling.gameObject.SetActive(active);
        fading.gameObject.SetActive(active);
    }

    private void UpdateVisuals()
    {
        float value = easeInAndOut.Evaluate(current);
        fading.color = new Color(1, 1, 1, value);
        scaling.localScale = new Vector3(value, value, value);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmojiObject : MonoBehaviour
{
    [SerializeField] TMP_Text textDisplay;
    [SerializeField] float duration = 3;
    [SerializeField] AnimationCurve localPositionOverTime, localScaleOverTime;

    private void Start()
    {
        Destroy(gameObject, duration);
    }
    public void Init(string text)
    {
        textDisplay.text = text;
        StartCoroutine(ChangeOverTimeRoutine());
    }

    private IEnumerator ChangeOverTimeRoutine()
    {
        float t = 0;
        
        while (t < 3f)
        {
            t += Time.deltaTime;
            transform.localPosition = localPositionOverTime.Evaluate(t) * Vector3.up;
            transform.localScale = localScaleOverTime.Evaluate(t) * Vector3.one;
            yield return null;
        }
    }
}

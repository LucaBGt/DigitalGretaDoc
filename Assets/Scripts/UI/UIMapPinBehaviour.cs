using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMapPinBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    float scaleMultiplier = 1;
    float parentScale = 0.6f;
    bool active;

    private void OnEnable()
    {
        GetComponentInParent<MinimapUI>().ChangeZoomEvent += OnZoomIn;
        UpdateScale();
    }

    private void OnDisable()
    {
        GetComponentInParent<MinimapUI>().ChangeZoomEvent -= OnZoomIn;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetActive(true);
    }

    private void SetActive(bool newActive)
    {
        active = newActive;
        StopAllCoroutines();
        StartCoroutine(AnimateScaleMultiplierRoutine(active ? 1.25f : 1f));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateScaleMultiplierRoutine(1.25f));
        UpdateScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateScaleMultiplierRoutine(1));
    }

    private IEnumerator AnimateScaleMultiplierRoutine(float target)
    {
        while (scaleMultiplier != target)
        {
            scaleMultiplier = Mathf.MoveTowards(scaleMultiplier, target, Time.deltaTime);
            UpdateScale();
            yield return null;
        }
    }

    private void OnZoomIn(float newParentScale)
    {
        parentScale = newParentScale;
        UpdateScale();
    }

    private void UpdateScale()
    {
        transform.localScale = Vector3.one *((1 / parentScale) * scaleMultiplier);
    }
}

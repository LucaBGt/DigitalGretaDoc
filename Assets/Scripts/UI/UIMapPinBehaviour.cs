using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMapPinBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] KeepAspectRatioRawImage logo;
    [SerializeField] Sprite defaultSprite, hoverSprite;

    float scaleMultiplier = 1;
    float parentScale = 0.6f;

    bool active;
    public bool Active => active;

    Image image;
    MinimapUI minimapUI;
    Door door;

    public UnityEvent onClick;

    public void Init(MinimapUI minimapUI, Door d)
    {

        if (d.Logo == null)
            logo.Hide();
        else
        {
            if (d.Logo.IsReady)
                logo.texture = d.Logo.Texture;
            else
            {
                logo.SetLoading();

                d.Logo.FinishedDownload += (request) =>
                {
                    logo.texture = request.Texture;
                };
            }
            //Logo texture is not ready and is currently being downloaded, should subscribe to DownloadFinished event and display a loading icon until the event is fired
        }

        door = d;


        image = GetComponent<Image>();
        this.minimapUI = minimapUI;
        minimapUI.ChangeZoomEvent += OnZoomIn;
        UpdateScale();
    }

    protected void OnDestroy()
    {
        if (minimapUI != null)
            minimapUI.ChangeZoomEvent -= OnZoomIn;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (active)
            minimapUI.Deselect();
        else
            minimapUI.Select(this, door);
        //onClick?.Invoke();
    }

    public void SetActive(bool newActive)
    {
        active = newActive;
        image.sprite = active ? hoverSprite : defaultSprite;
        StopAllCoroutines();
        StartCoroutine(AnimateScaleMultiplierRoutine(active ? 1.5f : 1f));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateScaleMultiplierRoutine(1.25f));
        UpdateScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (active)
            return;

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
        transform.localScale = Vector3.one * ((2 + (0.66f / parentScale)) * scaleMultiplier);
    }
}

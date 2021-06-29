using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class KeepAspectRatioRawImage : AspectRatioFitter
{
    protected RawImage rawImage;

    [SerializeField] LoadingUI loading;
    public Texture texture
    {
        get => rawImage.texture;
        set => SetRawImage(value);
    }

    public void SetLoading(bool loading = true)
    {
        CheckReferenceForRawImage();

        if (loading)
            rawImage.color = Color.clear;

        if (this.loading != null)
        {
            this.loading.enabled = loading;
            return;
        }

        Debug.LogWarning($"No loading ui defined. tried to set display loading ui: {loading}");
    }

    private void SetRawImage(Texture texture)
    {
        CheckReferenceForRawImage();

        rawImage.texture = texture;
        rawImage.color = Color.white;

        if (texture == null)
            return;

        SetLoading(false);
        CheckReferenceForRawImage();
        UpdateAspectRatio();
    }

    internal void Hide()
    {
        throw new NotImplementedException();
    }

    protected override void OnEnable()
    {
        CheckReferenceForRawImage();
        base.OnEnable();
    }

    private void CheckReferenceForRawImage()
    {
        if (rawImage != null)
            return;

        rawImage = GetComponent<RawImage>();
    }

    private void UpdateAspectRatio()
    {
        aspectRatio = (float)rawImage.texture.width / (float)rawImage.texture.height;
    }
}

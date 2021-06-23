using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class KeepAspectRatio : AspectRatioFitter
{
    RawImage rawImage;
    private bool update;

    protected override void OnEnable()
    {
        rawImage = GetComponent<RawImage>();
        base.OnEnable();
    }

    public void UpdateAspectRatio()
    {
        aspectRatio = aspectMode == AspectMode.HeightControlsWidth ?
            (float)rawImage.texture.width / (float)rawImage.texture.height :
            (float)rawImage.texture.height / (float)rawImage.texture.width;
    }
}

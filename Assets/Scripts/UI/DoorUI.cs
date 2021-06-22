using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorUI : ScalingUIElement
{
    [SerializeField] TMP_Text bigText, smallText;
    [SerializeField] RawImage logoImage;
    [SerializeField] RawImage imageBig, image01, image02, image03;
    [SerializeField] SocialMediaButton socialMediaButtonPrefab;
    [SerializeField] RectTransform socialMediaButtonParent;
    public void SetActiveTransition(bool active, Door currentDoor)
    {
        if (active && currentDoor != null)
        {
            UpdateContent(currentDoor.Data);
        }
        SetActiveTransition(active);
    }

    private void UpdateContent(RuntimeVendorData data)
    {
        bigText.text = data.InternalData.Name;
        smallText.text = data.InternalData.Description;
        logoImage.texture = data.LogoTexture;
        imageBig.texture = data.MainImageTexture;
        image01.texture = data.SubImagesTextures[0];
        image02.texture = data.SubImagesTextures[1];
        image03.texture = data.SubImagesTextures[2];
    }
}

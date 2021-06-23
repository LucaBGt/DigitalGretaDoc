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
    [SerializeField] RawImage imageBig;
    [SerializeField] RawImage[] imagesSmall;
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
        for (int i = 0; i < data.SubImagesTextures.Length; i++)
        {
            imagesSmall[i].texture = data.SubImagesTextures[i];
            if (i == 2)
                break;
        }
    }
}

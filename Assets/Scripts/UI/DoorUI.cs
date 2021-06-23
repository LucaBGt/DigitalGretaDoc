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

    KeepAspectRatio[] keepAspectRatios;

    private void OnEnable()
    {
        keepAspectRatios = GetComponentsInChildren<KeepAspectRatio>();
    }
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
        for (int i = 0; i < data.SubImagesTextures.Length && i < 3; i++)
        {
            imagesSmall[i].texture = data.SubImagesTextures[i];
        }

        foreach (KeepAspectRatio keepAspectRatio in keepAspectRatios)
        {
            keepAspectRatio.UpdateAspectRatio();
        }

        foreach (Transform child in socialMediaButtonParent)
        {
            Destroy(gameObject);
        }

        TryCreateSocialMediaButton(SocialMediaType.Facebook, data.InternalData.LinkFacebook);
        TryCreateSocialMediaButton(SocialMediaType.Instagram, data.InternalData.LinkInstagram);
        TryCreateSocialMediaButton(SocialMediaType.Homepage, data.InternalData.LinkWebsite);
        TryCreateSocialMediaButton(SocialMediaType.Pinterest, data.InternalData.LinkPinterest);
        TryCreateSocialMediaButton(SocialMediaType.YouTube, data.InternalData.LinkYouTube);
    }

    private void TryCreateSocialMediaButton(SocialMediaType type, string url)
    {
        if (!string.IsNullOrEmpty(url))
            Instantiate(socialMediaButtonPrefab, socialMediaButtonParent).Init(type, url);
    }
}

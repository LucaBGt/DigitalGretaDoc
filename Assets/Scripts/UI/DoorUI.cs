using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorUI : ScalingUIElement
{
    [SerializeField] TMP_Text bigText, smallText;
    [SerializeField] KeepAspectRatioRawImage logoImage;
    [SerializeField] KeepAspectRatioRawImage imageBig;
    [SerializeField] KeepAspectRatioRawImage[] imagesSmall;
    [SerializeField] SocialMediaButton socialMediaButtonPrefab;
    [SerializeField] RectTransform socialMediaButtonParent;
    [SerializeField] Button zoomMeetingButton;

    KeepAspectRatioRawImage[] keepAspectRatios = new KeepAspectRatioRawImage[0];

    Door currentDoor = null;

    private void OnEnable()
    {
        keepAspectRatios = GetComponentsInChildren<KeepAspectRatioRawImage>();
    }
    public void SetActiveTransition(bool active, Door doorInspected)
    {
        if (active && doorInspected != null)
        {
            currentDoor = doorInspected;
            UpdateContent(doorInspected.Data);
        }
        SetActiveTransition(active);
    }

    private void UpdateContent(RuntimeVendorData data)
    {
        bigText.text = data.InternalData.Name;
        smallText.text = data.InternalData.Description;

        UpdateTexture(currentDoor, data.Logo, logoImage);

        UpdateTexture(currentDoor, data.MainImage, imageBig);

        for (int i = 0; i < data.SubImages.Length && i < 3; i++)
        {
            UpdateTexture(currentDoor, data.SubImages[i], imagesSmall[i]);
        }

        foreach (Transform child in socialMediaButtonParent)
        {
            Destroy(child.gameObject);
        }

        TryCreateSocialMediaButton(SocialMediaType.Facebook, data.InternalData.LinkFacebook);
        TryCreateSocialMediaButton(SocialMediaType.Instagram, data.InternalData.LinkInstagram);
        TryCreateSocialMediaButton(SocialMediaType.Homepage, data.InternalData.LinkWebsite);
        TryCreateSocialMediaButton(SocialMediaType.Pinterest, data.InternalData.LinkPinterest);
        TryCreateSocialMediaButton(SocialMediaType.YouTube, data.InternalData.LinkYouTube);


        if (VendorsHander.Instance.ShowZoomLinks && !string.IsNullOrEmpty(data.InternalData.LinkZoom))
        {
            zoomMeetingButton.gameObject.SetActive(true);
            zoomMeetingButton.onClick.RemoveAllListeners();
            zoomMeetingButton.onClick.AddListener(() => GretaUtil.OpenURL(data.InternalData.LinkZoom));
        }
        else
        {
            zoomMeetingButton.gameObject.SetActive(false);
        }

    }

    private void UpdateTexture(Door door, TextureRequest req, KeepAspectRatioRawImage target)
    {

        if (req.IsReady)
        {
            target.texture = req.Texture;
        }
        else
        {
            //Loading icon should be displayed
            target.SetLoading();
            //If the texture is not ready we que up an UpdateContent call on finished download only if this is still the relevant door
            req.FinishedDownload += (req) =>
            {
                if (door == currentDoor)
                    UpdateTexture(door, req, target);
            };
        }
    }


    private void TryCreateSocialMediaButton(SocialMediaType type, string url)
    {
        if (!string.IsNullOrEmpty(url))
            Instantiate(socialMediaButtonPrefab, socialMediaButtonParent).Init(type, url);
    }
}

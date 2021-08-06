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

        for (int i = 0; i < SocialMediaButton.SOCIALMEDIA_COUNT; i++)
        {
            var type = (SocialMediaType)i;

            TryCreateSocialMediaButton(type, data.InternalData.GetLink(type));
        }


        if (VendorsHander.Instance.ShowZoomLinks && !string.IsNullOrEmpty(data.InternalData.GetLink(SocialMediaType.Zoom)))
        {
            zoomMeetingButton.gameObject.SetActive(true);
            zoomMeetingButton.onClick.RemoveAllListeners();
            zoomMeetingButton.onClick.AddListener(() => GretaUtil.OpenURL(data.InternalData.GetLink(SocialMediaType.Zoom)));
        }
        else
        {
            zoomMeetingButton.gameObject.SetActive(false);
        }

    }

    public void FullscreenImage(int index)
    {
        Texture toFullscreen = imagesSmall[index].texture;
        imagesSmall[index].texture = imageBig.texture;
        imageBig.texture = toFullscreen;
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

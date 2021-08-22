using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] UIMapPinBehaviour doorIndicatorPrefab;
    [SerializeField] RectTransform mapTopLeft, mapBotRight;
    [SerializeField] Transform realTopLeft, realBotRight;
    [SerializeField] Transform playerIndicator;
    [SerializeField] RectTransform doorIndicatorParent;

    [SerializeField] RectTransform mapRect;

    [Foldout("DetailWindow")] [SerializeField] TMP_Text nameText;
    [Foldout("DetailWindow")] [SerializeField] KeepAspectRatioRawImage logoImage;
    [Foldout("DetailWindow")] [SerializeField] TMP_Text describtionText;
    [Foldout("DetailWindow")] [SerializeField] Button infoButton;
    //[Foldout("DetailWindow")] [SerializeField] Button meetingButton;

    UIMapPinBehaviour[] indicators;
    float zoomFactor = 0.6f;
    Vector2 offset;
    private Door currentDoorInspected = null;
    public System.Action<float> ChangeZoomEvent;
    Coroutine moveMapCoroutine;


    private void Start()
    {
        offset = mapRect.localPosition;
        UpdateDetailWindow(null);
        PopulateMinimap();
        //meetingButton.gameObject.SetActive(false);
    }

    private void PopulateMinimap()
    {
        Door[] doors = FindObjectsOfType<Door>().OrderBy(d => d.transform.position.x).ToArray();
        List<UIMapPinBehaviour> pins = new List<UIMapPinBehaviour>();

        for (int i = 0; i < doors.Length; i++)
        {
            UIMapPinBehaviour indicator = Instantiate(doorIndicatorPrefab, doorIndicatorParent);
            indicator.Init(this, doors[i]);
            pins.Add(indicator);
            RectTransform rectTransform = indicator.GetComponent<RectTransform>();
            rectTransform.position = WorldToMinimap(doors[i].transform.position);
        }

        indicators = pins.ToArray();
    }

    internal void Select(UIMapPinBehaviour activePin, Door door)
    {
        foreach (UIMapPinBehaviour pin in indicators)
        {
            bool active = pin == activePin;
            if (pin.Active != active)
                pin.SetActive(active);
        }

        UpdateDetailWindow(door);
    }

    public void Deselect()
    {
        foreach (UIMapPinBehaviour pin in indicators)
        {
            pin.SetActive(false);
        }

        UpdateDetailWindow(null);
    }

    private void UpdateDetailWindow(Door door)
    {
        currentDoorInspected = door;
        bool show = (door != null && door.Data != null);

        infoButton.gameObject.SetActive(show);
        //meetingButton.gameObject.SetActive(show);

        if (show)
        {
            nameText.text = door.CompanyName;

            if (door.Logo.IsReady)
            {
                logoImage.texture = door.Logo.Texture;
            }
            else
            {
                var logo = door.Logo;
                var thisDoor = door;
                logoImage.SetLoading();
                door.Logo.FinishedDownload += (logo) =>
                {
                    if (currentDoorInspected == thisDoor)
                        logoImage.texture = logo.Texture;

                };
            }

            string describtion = door.CompanyDescription;
            describtionText.text = (describtion.Length > 250) ? (door.CompanyDescription.Substring(0, 250) + " ...") : describtion;
            infoButton.onClick.RemoveAllListeners();
            infoButton.onClick.AddListener(door.EnterInteractionFromMap);

            /*
            if (VendorsHander.Instance.ShowZoomLinks)
            {
                meetingButton.gameObject.SetActive(true);
                meetingButton.onClick.RemoveAllListeners();
                meetingButton.onClick.AddListener(door.OpenURL);
            }
            else
            {
                meetingButton.gameObject.SetActive(false);
            }
            */
        }
        else
        {
            nameText.text = "";
            describtionText.text = "Klicke auf einen Stand um dir seine Informationen anzusehen.";
            logoImage.texture = null;
            infoButton.onClick.RemoveAllListeners();
            //meetingButton.onClick.RemoveAllListeners();
        }
    }

    private void Update()
    {
        playerIndicator.position = WorldToMinimap(LocalPlayerBehaviour.Instance.transform.position);
    }

    private Vector2 WorldToMinimap(Vector3 p)
    {
        //Change reference frame to realTopLeft with y0
        p = p - realTopLeft.position;
        p.y = 0;

        float realWidth = realBotRight.position.x - realTopLeft.position.x;
        float realHeight = realTopLeft.position.z - realBotRight.position.z;

        float xAmount = p.x / realHeight;
        float yAmount = p.z / realWidth;

        float mapRight = mapBotRight.position.x - mapTopLeft.position.x;
        float mapUp = mapTopLeft.position.y - mapBotRight.position.y;

        Vector2 finalPos = mapTopLeft.position + new Vector3(mapUp * yAmount, mapRight * xAmount);

        return finalPos;
    }

    public void ZoomIn()
    {
        StartCoroutine(ChangeZoom(0.1f));
    }


    public void ZoomOut()
    {
        StartCoroutine(ChangeZoom(-0.1f));
    }

    public void MoveUp()
    {
        if (moveMapCoroutine != null)
            StopCoroutine(moveMapCoroutine);
        moveMapCoroutine = StartCoroutine(ChangePosition(new Vector2(0, -1)));
    }

    public void MoveDown()
    {
        if (moveMapCoroutine != null)
            StopCoroutine(moveMapCoroutine);
        moveMapCoroutine = StartCoroutine(ChangePosition(new Vector2(0, 1)));
    }
    public void MoveRight()
    {
        if (moveMapCoroutine != null)
            StopCoroutine(moveMapCoroutine);
        moveMapCoroutine = StartCoroutine(ChangePosition(new Vector2(-1, 0)));
    }

    public void MoveLeft()
    {
        if (moveMapCoroutine != null)
            StopCoroutine(moveMapCoroutine);
        moveMapCoroutine = StartCoroutine(ChangePosition(new Vector2(1, 0)));
    }

    private IEnumerator ChangeZoom(float change)
    {
        float zoomFactorBefore = zoomFactor;
        while (change < 0 ? (zoomFactor > (zoomFactorBefore + change)) : (zoomFactor < zoomFactorBefore + change))
        {
            zoomFactor += change * Time.deltaTime * 2;
            mapRect.localScale = Vector3.one * zoomFactor;
            ChangeZoomEvent?.Invoke(zoomFactor);
            yield return null;
        }
    }
    private IEnumerator ChangePosition(Vector2 change)
    {
        Vector2 offsetBefore = offset;
        Vector2 offsetAfter = offsetBefore + change * 50;

        float factor = 0;

        while (factor < 0.5f)
        {
            factor += Time.deltaTime;
            offset = Vector2.Lerp(offsetBefore, offsetAfter, factor * 2);
            mapRect.localPosition = offset;
            yield return null;
        }
    }
}

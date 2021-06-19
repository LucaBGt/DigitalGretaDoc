using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapUI : MonoBehaviour
{

    [SerializeField] MinimapDoorIndicator doorIndicatorPrefab;
    [SerializeField] RectTransform mapTopLeft, mapBotRight;
    [SerializeField] Transform realTopLeft, realBotRight;
    [SerializeField] Transform playerIndicator;

    [SerializeField] RectTransform mapRect;

    MinimapDoorIndicator[] indicators;
    float zoomFactor = 0.6f;
    public System.Action<float> ChangeZoomEvent;


    private void Start()
    {
        PopulateMinimap();
    }

    private void PopulateMinimap()
    {
        Door[] doors = FindObjectsOfType<Door>();

        for (int i = 0; i < doors.Length; i++)
        {
            MinimapDoorIndicator indicator = Instantiate(doorIndicatorPrefab, transform);
            indicator.Setup(doors[i]);
            RectTransform rectTransform = indicator.GetComponent<RectTransform>();
            rectTransform.position = WorldToMinimap(doors[i].transform.position);
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
}

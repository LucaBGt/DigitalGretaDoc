using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapUI : MonoBehaviour
{

    [SerializeField] MinimapDoorIndicator doorIndicatorPrefab;
    [SerializeField] RectTransform mapTopLeft, mapBotRight;
    [SerializeField] Transform realTopLeft, realBotRight;

    MinimapDoorIndicator[] indicators;


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

    private Vector2 WorldToMinimap(Vector3 p)
    {
        //Change reference frame to realTopLeft with y0
        p = p - realTopLeft.position;
        p.y = 0;

        Vector3 realRight = new Vector3(realBotRight.position.x - realTopLeft.position.x, 0, 0);
        Vector3 realFwd = new Vector3(0, 0, realTopLeft.position.z - realBotRight.position.z);

        float xAmount = Vector3.Dot(realRight.normalized, p);
        float yAmount = Vector3.Dot(realFwd.normalized, p);

        Vector2 mapRight = new Vector2(mapBotRight.position.x, -mapTopLeft.position.x);
        Vector2 mapUp = new Vector2(0, mapTopLeft.position.y - mapBotRight.position.y);

        Vector2 finalPos = (Vector2)mapTopLeft.position
            + mapRight.normalized * xAmount + mapUp.normalized * yAmount;

        return finalPos;
    }
}

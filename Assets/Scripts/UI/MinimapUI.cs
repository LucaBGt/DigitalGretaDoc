using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapUI : MonoBehaviour
{

    [SerializeField] MinimapDoorIndicator doorIndicatorPrefab;

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
        }
    }

}

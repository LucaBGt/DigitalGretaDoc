using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsInitializer : MonoBehaviour
{


    Door[] doors;

    private void Start()
    {
        doors = FindObjectsOfType<Door>();

        VendorsHander.Instance.Ready += OnVendorsHandlerReady;

    }

    private void OnVendorsHandlerReady()
    {
        int count = VendorsHander.Instance.VendorsCount;
        Debug.Log($"DoorsInitializer: Setting up {doors.Length} doors with {count} vendors.");

        if(count <= 0)
        {
            Debug.LogError("No vendors present. Cannot setup doors correctly");
            return;
        }

        for (int i = 0; i < doors.Length; i++)
        {
            if (i < count)
            {
                doors[i].Setup(VendorsHander.Instance.GetRuntimeVendorData(i));
            }
            else
            {
                //doors[i].Setup(VendorsHander.Instance.GetRuntimeVendorData(i%count));
                Destroy(doors[i].gameObject);
            }
        }

    }
}

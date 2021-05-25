using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugActivator : MonoBehaviour
{
    public GameObject Object;
    public void ActivateObject()
    {
        Object.SetActive(true);
        Destroy(this);
    }
}

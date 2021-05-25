using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCollider : MonoBehaviour
{
    public GameObject Object;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Object.SetActive(true);
        }
    }
}

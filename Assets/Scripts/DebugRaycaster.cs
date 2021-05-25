using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRaycaster : MonoBehaviour
{
    // Update is called once per frame

    public LayerMask layerMask;
    void Update()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.red);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log(hit.collider.gameObject.name + " is hit!");

            if (hit.collider.gameObject.CompareTag("LookAt"))
            {
                Debug.Log(hit.collider.gameObject.name + " is activated!");

                try
                {
                    DebugActivator da = hit.collider.gameObject.GetComponent<DebugActivator>();
                    da.ActivateObject();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
                //Destroy(this);
            }
        }
    }
}

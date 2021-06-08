using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera cam;
    private void OnEnable()
    {
        cam = Camera.main;
    }
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.down, cam.transform.position - transform.position);
    }
}

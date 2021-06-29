using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : Image
{
    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, Time.time * -360f);
    }
}

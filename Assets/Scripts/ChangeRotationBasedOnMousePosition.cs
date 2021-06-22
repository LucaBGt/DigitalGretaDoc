using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRotationBasedOnMousePosition : MonoBehaviour
{
    [SerializeField] AnimationCurve verticalRotationCurve = AnimationCurve.Linear(0,1,1,0);
    [SerializeField] AnimationCurve horizontalRotationCurve = AnimationCurve.Linear(0,0,1,1);
    // Update is called once per frame
    void Update()
    {
        float x = Input.mousePosition.x / Screen.width;
        float y = Input.mousePosition.y / Screen.height;
        transform.localRotation = Quaternion.Euler(verticalRotationCurve.Evaluate(y), horizontalRotationCurve.Evaluate(x), 0);
    }
}

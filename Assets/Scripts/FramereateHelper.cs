using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramereateHelper : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = -1;
    }
}

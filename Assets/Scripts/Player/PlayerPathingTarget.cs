using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathingTarget : MonoBehaviour
{
    
    [SerializeField] SpriteRenderer visualization;

    [SerializeField] Transform subGoalTransform;

    [SerializeField] Color specialColor;

    public void SetTargetSimple(Vector3 target)
    {
        gameObject.SetActive(true);
        subGoalTransform.gameObject.SetActive(false);
        visualization.color = Color.white;
        transform.position = target;
    }

    public void SetTargetSpecial(Vector3 target, Vector3 secondaryTarget)
    {
        gameObject.SetActive(true);
        subGoalTransform.gameObject.SetActive(true);
        visualization.color = specialColor;
        transform.position = target;
        subGoalTransform.position = secondaryTarget;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

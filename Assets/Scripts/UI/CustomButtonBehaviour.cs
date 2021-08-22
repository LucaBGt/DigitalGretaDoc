using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButtonBehaviour : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onClick;
    internal bool interactable = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (interactable) onClick?.Invoke();
    }

}

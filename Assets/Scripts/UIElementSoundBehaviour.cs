using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementSoundBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] AudioClip clip;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundPlayer.Instance.PlayClipWithPitch(clip, 0.5f, volume: 0.2f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundPlayer.Instance.PlayClipWithPitch(clip, 1f, volume: 0.2f);
    }
}

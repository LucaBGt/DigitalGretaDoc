using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ICancallableInteractable
{
    private static readonly int ANIM_OpenDoor = Animator.StringToHash("OpenDoor");

    [SerializeField] Transform goalPosition;

    [SerializeField] string companyName;
    [SerializeField] string url;
    [SerializeField] Sprite logo;
    [SerializeField] CinemachineVirtualCamera vcam;

    [SerializeField] AudioClip openDoor, closeDoor;

    Animator animator;


    public event Action Cancel;

    public string CompanyName => companyName;
    public Sprite Logo => logo;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void EnterInteraction()
    {
        UIHandler.Instance.OpenDoor(this);
        vcam.Priority = 20;
    }

    public void ExitInteraction()
    {
        UIHandler.Instance.CloseDoor(this);
        vcam.Priority = 5;
    }

    public Vector3 GetInteractPosition()
    {
        return goalPosition.position;
    }

    public void OpenURL()
    {
        if (!string.IsNullOrEmpty(url))
            Application.OpenURL(url);
    }

    public void CancelInteraction()
    {
        Cancel?.Invoke();
    }

    public float GetInteractYRotation()
    {
        Vector3 dir = transform.position - goalPosition.position;
        Vector3 rot = Quaternion.FromToRotation(Vector3.forward, dir).eulerAngles;
        return rot.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        animator.SetBool(ANIM_OpenDoor, true);
        SoundPlayer.Instance.Play(openDoor, source3D: transform, volume: 0.25f, randomPitchRange: 0.1f);
    }

    private void OnTriggerExit(Collider other)
    {
        animator.SetBool(ANIM_OpenDoor, false);
        SoundPlayer.Instance.Play(closeDoor, source3D: transform, volume: 0.25f, randomPitchRange: 0.1f);
    }
}
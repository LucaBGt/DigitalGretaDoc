using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private static readonly int ANIM_OpenDoor = Animator.StringToHash("OpenDoor");

    [SerializeField] Transform goalPosition;

    Animator animator;

    public string url;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowAnimation()
    {
        animator.SetBool("OpenDoor", true);
    }

    public void EndDoor()
    {
        animator.SetBool("OpenDoor", false);
    }

    public void EnterInteraction()
    {
        animator.SetBool(ANIM_OpenDoor,true);
    }

    public void ExitInteraction()
    {
        animator.SetBool(ANIM_OpenDoor, false);
    }

    public Vector3 GetInteractPosition()
    {
        return goalPosition.position;
    }

    public float GetInteractYRotation()
    {
        Vector3 dir = transform.position - goalPosition.position;
        Vector3 rot = Quaternion.FromToRotation(Vector3.forward, dir).eulerAngles;
        return rot.y;
    }


}

public class stand_json
{
    public string zoom_url;
    public string image_url;
}
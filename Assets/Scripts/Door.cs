using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] Transform goalPosition;

    Animator animator;

    public string url;

    public Vector3 GoalPosition { get => goalPosition.position; } 

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

    public void Interact()
    {
        Debug.Log("Opened door");
    }
}

public class stand_json
{
    public string zoom_url;
    public string image_url;
}
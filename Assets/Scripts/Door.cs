using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
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
}

public class stand_json
{
    public string zoom_url;
    public string image_url;
}
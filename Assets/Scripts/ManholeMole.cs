using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManholeMole : MonoBehaviour
{
    Animator animator;
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("hide", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        animator.SetBool("hide", true);
    }

    private void OnTriggerExit(Collider other)
    {
        animator.SetBool("hide", false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationHandler : MonoBehaviour
{
    Animator animator;
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        GetComponent<PlayerBehaviour>().OnChangePlayerState += ChangePlayerState;
    }

    private void OnDisable()
    {
        GetComponent<PlayerBehaviour>().OnChangePlayerState -= ChangePlayerState;
    }
    private void ChangePlayerState(PlayerState state)
    {
        if (animator == null)
            return;

        switch (state)
        {
            case PlayerState.Walking:
                animator.SetTrigger("Walk");
                break;

            default:
                animator.SetTrigger("Idle");
                break;
        }
    }
}

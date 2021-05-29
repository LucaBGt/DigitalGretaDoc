using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] NetworkedPlayerBehaviour networkedPlayer;

    Animator animator;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        networkedPlayer.NetworkedPlayerStateChanged += ChangePlayerState;
    }

    private void OnDisable()
    {
        if (networkedPlayer != null)
            networkedPlayer.NetworkedPlayerStateChanged -= ChangePlayerState;
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

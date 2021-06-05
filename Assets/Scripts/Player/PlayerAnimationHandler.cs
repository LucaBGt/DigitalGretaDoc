using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationHandler : MonoBehaviour
{
    private static readonly int ANIM_Walk = Animator.StringToHash("Walk");
    private static readonly int ANIM_Idle = Animator.StringToHash("Idle");

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
                animator.SetTrigger(ANIM_Walk);
                break;

            default:
                animator.SetTrigger(ANIM_Idle);
                break;
        }
    }
}

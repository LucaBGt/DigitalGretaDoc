using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationHandler : MonoBehaviour
{
    private static readonly int ANIM_Walk = Animator.StringToHash("Walk");
    private static readonly int ANIM_Idle = Animator.StringToHash("Idle");

    [SerializeField] IPlayerBehaviour player;

    Animator animator;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<IPlayerBehaviour>();
        player.PlayerStateChanged += ChangePlayerState;
    }

    private void OnDisable()
    {
        if (player != null)
            player.PlayerStateChanged -= ChangePlayerState;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private static readonly int ANIM_Walk = Animator.StringToHash("Walk");

    [SerializeField] IPlayerBehaviour player;

    Animator animator;

    private void OnEnable()
    {
        ReselectAnimator();
        player = GetComponent<IPlayerBehaviour>();
        player.PlayerStateChanged += ChangePlayerState;
    }

    private void OnDisable()
    {
        if (player != null)
            player.PlayerStateChanged -= ChangePlayerState;
    }

    public void ReselectAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void ChangePlayerState(PlayerState state)
    {
        if (animator == null)
            return;

        switch (state)
        {
            case PlayerState.Walking:
                animator.SetBool(ANIM_Walk, true);
                break;

            default:
                animator.SetBool(ANIM_Walk, false);
                break;
        }
    }
}

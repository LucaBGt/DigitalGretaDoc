using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public interface IPlayerBehaviour
{
    event System.Action<PlayerState> PlayerStateChanged;
}

public class NetworkedPlayerBehaviour : NetworkBehaviour, IPlayerBehaviour
{
    [SerializeField] GameObject onDestroyEffect;
    [SerializeField] GameObject playerVisualsObject;

    [Header("Player Name")]
    [SerializeField] TextMeshPro playerNameText;
    [SerializeField] Renderer playerSkin;

    Camera cam;

    public event System.Action<PlayerState> PlayerStateChanged;


    private void Start()
    {
        cam = Camera.main;
    }


    private void OnLocalChangePlayerState(PlayerState obj)
    {
        CMD_ChangePlayerState(obj);
    }

    [Command]
    public void CMD_ChangePlayerState(PlayerState obj)
    {
        RPC_ChangePlayerState(obj);
    }

    [ClientRpc]
    public void RPC_ChangePlayerState(PlayerState newState)
    {
        PlayerStateChanged?.Invoke(newState);
    }

    [Command]
    private void CMD_SetupRemotePlayer(string _name, int skinId)
    {
        // player info sent to server, then server updates sync vars which handles it on all clients
        RPC_SetupRemotePlayer(_name, skinId);
    }


    [ClientRpc]
    private void RPC_SetupRemotePlayer(string _name, int skinId)
    {
        LocalPlayerBehaviour.SetupLocalPlayerVisuals(playerNameText, playerSkin, _name, skinId);
    }


    public override void OnStartLocalPlayer()
    {
        CMD_SetupRemotePlayer(Settings.Instance.Username, Settings.Instance.UserSkinID);

        LocalPlayerBehaviour.Instance.PlayerStateChanged += OnLocalChangePlayerState;

        Destroy(playerVisualsObject);
        Destroy(GetComponent<PlayerAnimationHandler>());
        Destroy(GetComponent<Animator>());
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
            LocalPlayerBehaviour.Instance.PlayerStateChanged -= OnLocalChangePlayerState;

        if (onDestroyEffect != null && !GameInstance.ApplicationQuitting)
        {
            Instantiate(onDestroyEffect, transform.position, transform.rotation);
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            //teleport to local player location
            var player = LocalPlayerBehaviour.Instance.transform;
            transform.position = player.position;
            transform.rotation = player.rotation;
        }
        else
        {
            //remote player update
            playerNameText.transform.forward = transform.position - cam.transform.position;
        }
    }
}

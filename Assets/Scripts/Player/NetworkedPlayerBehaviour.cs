using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class NetworkedPlayerBehaviour : NetworkBehaviour
{
    [SerializeField] GameObject onDestroyEffect;

    [Header("Player Name")]
    [SerializeField] TextMeshPro playerNameText;
    [SerializeField] Renderer playerSkin;

    Camera cam;

    public event System.Action<PlayerState> NetworkedPlayerStateChanged;


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
        NetworkedPlayerStateChanged?.Invoke(newState);
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
        playerNameText.text = _name;

        if (skinId < Settings.Instance.SkinsCount)
        {
            playerSkin.sharedMaterial = Settings.Instance.SkinsMaterials[skinId];
        }
        else
        {
            Debug.LogWarning("Passed skinID not present in this version. Selecting default");
            playerSkin.sharedMaterial = Settings.Instance.SkinsMaterials[0];
        }
    }


    public override void OnStartLocalPlayer()
    {
        CMD_SetupRemotePlayer(Settings.Instance.Username, Settings.Instance.UserSkinID);

        LocalPlayerBehaviour.Instance.ChangePlayerState += OnLocalChangePlayerState;
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
            LocalPlayerBehaviour.Instance.ChangePlayerState -= OnLocalChangePlayerState;

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
        }
        playerNameText.transform.forward = transform.position - cam.transform.position;
    }
}

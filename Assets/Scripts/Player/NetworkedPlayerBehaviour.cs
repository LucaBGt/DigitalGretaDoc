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
    [SerializeField] PlayerAnimationHandler animationHandler;

    [Header("Player Name")]
    [SerializeField] TextMeshPro playerNameText;

    Camera cam;
    GameObject currentVisuals;

    public event System.Action<PlayerState> PlayerStateChanged;


    [SyncVar(hook = nameof(SyncVarHook_SkinChanged))]
    private int sv_skinID = -1;
    [SyncVar(hook = nameof(SyncVarHook_UsernameChanged))]
    private string sv_username;

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
        sv_skinID = skinId;
        sv_username = _name;
    }

    private void SyncVarHook_UsernameChanged(string _oldname, string _newname)
    {
        //Debug.Log(nameof(SyncVarHook_UsernameChanged));
        if (!isLocalPlayer)
            playerNameText.text = _newname;
    }

    private void SyncVarHook_SkinChanged(int _oldId, int _newID)
    {
        Debug.Log(name + " skin changed to " + _newID);
        if (!isLocalPlayer)
        {
            LocalPlayerBehaviour.SetupLocalPlayerVisuals(ref currentVisuals, transform, sv_skinID);
            animationHandler.ReselectAnimator();
        }
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer");
        CMD_SetupRemotePlayer(Settings.Instance.Username, Settings.Instance.UserSkinID);

        LocalPlayerBehaviour.Instance.PlayerStateChanged += OnLocalChangePlayerState;

        Destroy(GetComponent<PlayerAnimationHandler>());
        Destroy(playerNameText.gameObject);
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
            if (cam != null)
                playerNameText.transform.forward = transform.position - cam.transform.position;
        }
    }
}

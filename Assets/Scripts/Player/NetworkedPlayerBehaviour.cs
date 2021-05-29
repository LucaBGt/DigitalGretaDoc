using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class NetworkedPlayerBehaviour : NetworkBehaviour
{
    [Header("Player Name")]
    public TextMeshPro playerNameText;

    [SyncVar(hook = nameof(OnNameChanged))]
    [SerializeField] string playerName;

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
    public void CmdSetupPlayer(string _name)
    {
        // player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
    }

    void OnNameChanged(string _Old, string _New)
    {
        playerNameText.text = playerName;
    }

    public override void OnStartLocalPlayer()
    {
        string playerName = "Guest";
        CmdSetupPlayer(playerName);

        LocalPlayerBehaviour.Instance.ChangePlayerState += OnLocalChangePlayerState;
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
            LocalPlayerBehaviour.Instance.ChangePlayerState -= OnLocalChangePlayerState;
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
            playerNameText.transform.LookAt(cam.transform);
        }
    }
}

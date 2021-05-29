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

    void OnNameChanged(string _Old, string _New)
    {
        playerNameText.text = playerName;
    }

    private void Start()
    {
        cam = Camera.main;
    }

    public override void OnStartLocalPlayer()
    {
        string playerName = "Guest";
        CmdSetupPlayer(playerName);

        LocalPlayerBehaviour.Instance.ChangePlayerState += OnLocalChangePlayerState;
    }

    private void OnLocalChangePlayerState(PlayerState obj)
    {

    }

    [Command]
    public void CmdSetupPlayer(string _name)
    {
        // player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
    }


    void Update()
    {
        if (isLocalPlayer)
        {
            //teleport to local player location
        }
        else
        {
            //remote player update
            playerNameText.transform.LookAt(cam.transform);
        }
    }


}

using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiDiplayHandler : NetworkBehaviour
{
    [SerializeField] List<EmojiObject> emojis;
    [SerializeField] Transform parent;

    private static EmojiDiplayHandler instance;

    public static EmojiDiplayHandler Instance => instance;

    public override void OnStartLocalPlayer()
    {
        instance = this;
        Debug.Log("Setup local Emoji Display Handler");
    }

    internal void SpawnEmoji(int index)
    {
        CMD_SpawnEmoji(index);
    }

    [Command]
    private void CMD_SpawnEmoji(int index)
    {
        Debug.Log(nameof(CMD_SpawnEmoji));
        RPC_SpawnEmoji(index);
    }

    [ClientRpc]
    private void RPC_SpawnEmoji(int index)
    {
        EmojiObject prefab = emojis[index];
        EmojiObject emojiObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
    }
}

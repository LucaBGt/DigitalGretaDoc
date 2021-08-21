using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiDiplayHandler : NetworkBehaviour
{
    [SerializeField] List<EmojiObject> speechbubblePrefabs;
    [SerializeField] SpeechbubbleData speechbubbleData;
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
        Debug.Log($"Spawn Emoji ({index})");
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
        EmojiObject prefab = speechbubblePrefabs[UnityEngine.Random.Range(0, speechbubblePrefabs.Count)];
        EmojiObject emojiObject = Instantiate(prefab, parent);
        emojiObject.Init(speechbubbleData.SpeechbubbleTexts[index]);
    }
}

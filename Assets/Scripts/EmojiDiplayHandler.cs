using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiDiplayHandler : NetworkBehaviour
{
    [SerializeField] EmojiObject prefab;
    [SerializeField] Vector3 offset;
    [SerializeField] List<Sprite> emojis;
    [SerializeField] Transform parent;

    private static EmojiDiplayHandler instance;

    public static EmojiDiplayHandler Instance => instance;

    public override void OnStartLocalPlayer()
    {
        instance = this;
        Debug.Log("Setup local Emoji Display Handler");
    }

    public void SpawnEmoji(Vector3 playerPosition, Sprite emojiSprite)
    {
        int emjoiIndex = emojis.IndexOf(emojiSprite);
        CMD_SpawnEmoji(playerPosition, emjoiIndex);
    }

    [Command]
    private void CMD_SpawnEmoji(Vector3 playerPosition, int emojiSpriteIndex)
    {
        Debug.Log(nameof(CMD_SpawnEmoji));
        RPC_SpawnEmoji(playerPosition, emojiSpriteIndex);
    }

    [ClientRpc]
    private void RPC_SpawnEmoji(Vector3 playerPosition, int emojiSpriteIndex)
    {
        Sprite emojiSprite = emojis[emojiSpriteIndex];
        EmojiObject emojiObject = Instantiate(prefab, playerPosition + offset, Quaternion.identity, parent);
        emojiObject.SetSprite(emojiSprite);
    }
}

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiDiplayHandler : NetworkBehaviour
{
    [SerializeField] EmojiObject prefab;
    [SerializeField] Vector3 offset;
    [SerializeField] List<Sprite> emojis;

    private static EmojiDiplayHandler instance;

    public static EmojiDiplayHandler Instance => instance;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError($"Spawned Second Instance of {GetType()}, destroying");
            Destroy(gameObject);
        }
    }

    public void SpawnEmoji(Vector3 playerPosition, Sprite emojiSprite)
    {
        int emjoiIndex = emojis.IndexOf(emojiSprite);
        CMD_SpawnEmoji(playerPosition, emjoiIndex);
    }

    [Command]
    private void CMD_SpawnEmoji(Vector3 playerPosition, int emojiSpriteIndex)
    {
        RPC_SpawnEmoji(playerPosition, emojiSpriteIndex);
    }

    [ClientRpc]
    private void RPC_SpawnEmoji(Vector3 playerPosition, int emojiSpriteIndex)
    {
        Sprite emojiSprite = emojis[emojiSpriteIndex];
        EmojiObject emojiObject = Instantiate(prefab, playerPosition + offset, Quaternion.identity, transform);
        emojiObject.SetSprite(emojiSprite);
    }
}

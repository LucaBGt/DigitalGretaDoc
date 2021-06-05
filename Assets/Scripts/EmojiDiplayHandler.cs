using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiDiplayHandler : SingletonBehaviour<EmojiDiplayHandler>
{
    [SerializeField] EmojiObject prefab;
    [SerializeField] Vector3 offset;
    public void SpawnEmoji(Vector3 playerPosition, Sprite emojiSprite)
    {
        EmojiObject emojiObject = Instantiate(prefab, playerPosition + offset, Quaternion.identity, transform);
        emojiObject.SetSprite(emojiSprite);
    }
}

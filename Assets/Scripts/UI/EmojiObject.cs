using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiObject : MonoBehaviour
{
    [SerializeField] float duration = 3;

    private void Start()
    {
        Destroy(gameObject, duration);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmojiObject : MonoBehaviour
{
    [SerializeField] TMP_Text textDisplay;
    [SerializeField] float duration = 3;

    public void Init(string text)
    {
        textDisplay.text = text;
        Destroy(gameObject, duration);
    }
}

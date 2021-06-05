using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiUI : MonoBehaviour
{
    [SerializeField] RectTransform emojis;
    [SerializeField] Vector2 emojisSizeTarget;
    [SerializeField] AnimationCurve scaleUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float animationSpeed = 4f;

    float emojisScaleFactorCurrent = 0;
    float emojisScaleFactorTarget = 0;
    bool animate = false;

    public void Toggle()
    {
        emojisScaleFactorTarget = emojisScaleFactorTarget < 1 ? 1 : 0;
        animate = true;
    }

    public void SendEmoji(Sprite sprite)
    {
        Debug.Log($"Display Emoji{sprite.name}");
    }

    private void Update()
    {
        if (animate)
        {
            emojisScaleFactorCurrent = Mathf.Clamp(Mathf.MoveTowards(emojisScaleFactorCurrent, emojisScaleFactorTarget, Time.deltaTime * animationSpeed), 0f, 1f);

            if (Mathf.Abs(emojisScaleFactorCurrent - emojisScaleFactorTarget) < 0.01f)
                animate = false;

            SetEmojisWidthAndHeightByValue(scaleUpCurve.Evaluate(emojisScaleFactorCurrent));
        }
    }

    private void SetEmojisWidthAndHeightByValue(float value)
    {
        emojis.sizeDelta = Vector2.Lerp(Vector2.zero, emojisSizeTarget, value);
    }
}

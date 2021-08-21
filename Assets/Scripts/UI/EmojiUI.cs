using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmojiUI : MonoBehaviour
{
    [SerializeField] RectTransform emojis;
    [SerializeField] CanvasGroup emojiGroup;
    [SerializeField] Vector2 emojisSizeTarget;
    [SerializeField] AnimationCurve scaleUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float animationSpeed = 4f;
    [SerializeField] float useCooldown;
    [SerializeField] SpeechbubbleData speechbubbleTexts;
    [SerializeField] CustomButtonBehaviour speechbubbleButtonPrefab;

    List<CustomButtonBehaviour> emojiButtons;
    float emojisScaleFactorCurrent = 0;
    float emojisScaleFactorTarget = 0;
    bool animate = false;

    private void Start()
    {
        PopulateSpawnEmojis();
    }

    private void PopulateSpawnEmojis()
    {
        emojiButtons = new List<CustomButtonBehaviour>();

        for (int i = 0; i < speechbubbleTexts.SpeechbubbleTexts.Length; i++)
        {
            CustomButtonBehaviour button = Instantiate(speechbubbleButtonPrefab, emojiGroup.transform);
            button.transform.localScale = new Vector3(UnityEngine.Random.Range(0.9f, 1.1f), 1, 1);
            button.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-5f, 5f));
            button.GetComponentInChildren<TMP_Text>().text = speechbubbleTexts.SpeechbubbleTexts[i];
            int index = i;
            button.onClick.AddListener(delegate { SendEmoji(index); });
            emojiButtons.Add(button);
        }
    }

    public void Toggle()
    {
        emojisScaleFactorTarget = emojisScaleFactorTarget < 1 ? 1 : 0;
        animate = true;
    }

    public void SendEmoji(int index)
    {
        Debug.LogWarning("send index: " + index);

        EmojiDiplayHandler.Instance.SpawnEmoji(index);
        emojisScaleFactorTarget = 0;
        animate = true;
        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        emojiGroup.alpha = 0.5f;
        foreach (var button in emojiButtons)
        {
            button.interactable = false;
        }

        yield return new WaitForSeconds(useCooldown);
        emojiGroup.alpha = 1f;
        foreach (var button in emojiButtons)
        {
            button.interactable = true;
        }
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
        emojis.localScale = Vector2.Lerp(Vector2.zero, emojisSizeTarget, value);
    }
}

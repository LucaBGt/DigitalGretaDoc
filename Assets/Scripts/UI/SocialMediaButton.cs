using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SocialMediaType
{
    Homepage,
    Facebook,
    YouTube,
    Instagram,
    Pinterest,
    Zoom,
}

public class SocialMediaButton : CustomButtonBehaviour
{
    [SerializeField] SocialMediaTypeSpritePair[] typeSpritePairs;

    private string url;

    public void Init(SocialMediaType type, string url)
    {
        GetComponent<Image>().sprite = GetSpriteBySocialMediaType(type);
        this.url = url;
        onClick.AddListener(OpenUrl);
    }

    private void OnDestroy()
    {
        onClick.RemoveAllListeners();
    }

    private void OpenUrl()
    {
        GretaUtil.OpenURL(url);
    }

    private Sprite GetSpriteBySocialMediaType(SocialMediaType type)
    {
        foreach (SocialMediaTypeSpritePair pair in typeSpritePairs)
        {
            if (pair.Type == type)
                return pair.Sprite;
        }

        return null;
    }
}

[System.Serializable]
public class SocialMediaTypeSpritePair
{
    public SocialMediaType Type;
    public Sprite Sprite;
}

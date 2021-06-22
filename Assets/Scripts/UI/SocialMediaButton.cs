using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Sprite GetSpriteBySocialMediaType(SocialMediaType type)
    {
        Sprite sprite = null;
        return sprite;
    }
}

[System.Serializable]
public class SocialMediaTypeSpritePair
{
    public SocialMediaType Type;
    public Sprite Sprite;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialMediaButton : MonoBehaviour
{
    public string linklink;

    public void OpenLink()
    {
        Application.OpenURL(linklink);
    }
}

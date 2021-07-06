using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GretaUtil
{
    
    public static void OpenURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            if (!url.StartsWith("http"))
                url = "https://" + url;

            Debug.Log("Opening URL: " + url);
            Application.OpenURL(url);
        }
    }

}

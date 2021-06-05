using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : SingletonBehaviour<Settings>
{
    private string userName;
    private int userSkin;

    protected override void Awake()
    {
        userName = PlayerPrefs.GetString(nameof(userName));
        userSkin = PlayerPrefs.GetInt(nameof(userSkin));
    }


    private void OnDestroy()
    {
        PlayerPrefs.SetString(nameof(userName), userName);
        PlayerPrefs.SetInt(nameof(userSkin), userSkin);
        PlayerPrefs.Save();
    }
}

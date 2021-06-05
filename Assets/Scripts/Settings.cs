using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : SingletonBehaviour<Settings>
{
    [SerializeField] Texture2D[] skins;

    private string username = "Greta";
    private int userSkin;

    public int UserSkinID => userSkin;
    public string Username => username;

    public Texture2D[] Skins => skins;

    public int ChangeSkinID(int id)
    {
        userSkin = (id + skins.Length) % skins.Length;
        return userSkin;
    }

    public void SetUserName(string username)
    {
        this.username = username;
    }


    protected override void Awake()
    {
        base.Awake();
        username = PlayerPrefs.GetString(nameof(username));
        userSkin = PlayerPrefs.GetInt(nameof(userSkin));
    }


    private void OnDestroy()
    {
        PlayerPrefs.SetString(nameof(username), username);
        PlayerPrefs.SetInt(nameof(userSkin), userSkin);
        PlayerPrefs.Save();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : SingletonBehaviour<Settings>
{
    const string DEFAULT_USERNAME = "Gast";

    private GretaSettings generalSettings;

    private string username;
    private int userSkin;
    private float volume;

    public int UserSkinID => userSkin;
    public string Username => username;

    public float MasterVolume => volume;

    public int SkinsCount => generalSettings.Skins.Length;
    public Texture2D[] Skins => generalSettings.Skins;
    public Material[] SkinsMaterials => generalSettings.SkinsMaterials;

    public int ChangeSkinID(int id)
    {
        userSkin = (id + SkinsCount) % SkinsCount;
        return userSkin;
    }

    public void SetUserName(string username)
    {
        this.username = username;
    }

    protected override void Awake()
    {
        base.Awake();
        generalSettings = Resources.Load<GretaSettings>("Settings");
        if (generalSettings == null)
        {
            Debug.LogError("Failed to load Greta Settings!");
            generalSettings = ScriptableObject.CreateInstance<GretaSettings>();
        }

        username = PlayerPrefs.GetString(nameof(username));
        userSkin = PlayerPrefs.GetInt(nameof(userSkin));
        volume = PlayerPrefs.GetFloat(nameof(volume));

        if (string.IsNullOrEmpty(username))
            username = DEFAULT_USERNAME;
    }


    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
    }

    private void OnDestroy()
    {
        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetString(nameof(username), username);
        PlayerPrefs.SetInt(nameof(userSkin), userSkin);
        PlayerPrefs.SetFloat(nameof(volume), volume);
        PlayerPrefs.Save();
    }
}

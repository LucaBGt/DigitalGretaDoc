using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] UIHandler uiHandler;
    [SerializeField] RawImage characterTexture;
    [SerializeField] TMPro.TMP_InputField usernameInputField;
    int currentSkin;


    Settings settings;
    private void Start()
    {
        settings = Settings.Instance;
        currentSkin = settings.UserSkinID;
        usernameInputField.text = settings.Username;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        characterTexture.texture = settings.Skins[currentSkin];
    }

    public void ChangeCharacterRight()
    {
        currentSkin = settings.ChangeSkinID(currentSkin + 1);
        UpdateVisuals();
    }

    public void ChangeCharacterLeft()
    {
        currentSkin = settings.ChangeSkinID(currentSkin - 1);
        UpdateVisuals();
    }

    public void SetUsername(string newUsername)
    {
        settings.SetUserName(newUsername);
    }

    public void StartGame()
    {
        uiHandler.StartGame();
    }

    public void Back()
    {
        uiHandler.ReturnFromCharacterSelection();
    }

}

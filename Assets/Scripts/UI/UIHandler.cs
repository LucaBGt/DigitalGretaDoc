using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPerspectiveToggleUI
{
    void ToggleSwitchPerspective();
}

public interface IPlayerUI
{
    void StartTurnRight();
    void StartTurnLeft();
    void EndTurnRight();
    void EndTurnLeft();
}

public enum UIState
{
    InGame,
    InMainMenu,
    InCharacterSelection
}

public class UIHandler : SingletonBehaviour<UIHandler>, IPlayerUI, IPerspectiveToggleUI
{
    [SerializeField] GameObject doorUI;
    [SerializeField] GameObject stopButtonObject;
    [SerializeField] GameObject mainMenu, characterSelection, emojiUI;
    [SerializeField] bool skipMainMenuInEditor;

    Door currentDoor = null;
    UIState uiState;

    public bool InLockedUIMode => uiState != UIState.InGame;

    private void Start()
    {
        GetLocalPlayer().ChangePlayerState += OnPlayerStateChanged;

#if UNITY_EDITOR
        if (skipMainMenuInEditor)
            return;
#endif

        EnterMainMenu();

        GretaNetworkManager.Instance.ConnectionStateChanged += OnConnectionStateChanged;
    }

    private void OnDestroy()
    {
        if (GretaNetworkManager.Instance)
            GretaNetworkManager.Instance.ConnectionStateChanged -= OnConnectionStateChanged;
    }

    private void OnConnectionStateChanged(GretaConnectionState obj)
    {
        Debug.Log(obj);
        switch (obj)
        {
            case GretaConnectionState.Connected:
                emojiUI.SetActive(true);
                break;

            case GretaConnectionState.Disconnected:
                emojiUI.SetActive(false);
                break;
        }
    }

    private void OnPlayerStateChanged(PlayerState obj)
    {
        //stop not in looking or interaction
        stopButtonObject.SetActive(obj != PlayerState.Looking && obj != PlayerState.InInteraction);
    }

    public void EnterMainMenu()
    {
        mainMenu.SetActive(true);
        characterSelection.SetActive(false);
        uiState = UIState.InMainMenu;
    }

    public void EnterCharacterSelection()
    {
        mainMenu.SetActive(false);
        characterSelection.SetActive(true);
        uiState = UIState.InCharacterSelection;
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        characterSelection.SetActive(false);
        uiState = UIState.InGame;

        GretaNetworkManager.Instance?.GretaJoin();
    }

    public void OpenDoor(Door d)
    {
        doorUI.SetActive(true);
        currentDoor = d;
    }

    public void CloseDoor(Door d)
    {
        doorUI.SetActive(false);
        currentDoor = null;
    }

    public void ForceCloseDoor()
    {
        if (currentDoor != null)
        {
            currentDoor.CancelInteraction();
        }
    }


    public void StartTurnLeft()
    {
        if (InLockedUIMode) return;

        GetLocalPlayer().TurnLeft();
    }

    public void StartTurnRight()
    {
        if (InLockedUIMode) return;

        GetLocalPlayer().TurnRight();
    }
    public void EndTurnLeft()
    {
        GetLocalPlayer().StopTurning();
    }

    public void EndTurnRight()
    {
        GetLocalPlayer().StopTurning();
    }

    public void Stop()
    {
        GetLocalPlayer().Stop();
    }

    public void ToggleSwitchPerspective()
    {
        if (InLockedUIMode) return;

        GetLocalPlayer().TogglePerspective();
    }
    private LocalPlayerBehaviour GetLocalPlayer()
    {
        return LocalPlayerBehaviour.Instance;
    }
}

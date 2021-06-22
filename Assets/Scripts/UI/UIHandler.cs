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
    InCharacterSelection,
    InMinimap,
    VisitCard
}

public class UIHandler : SingletonBehaviour<UIHandler>, IPlayerUI, IPerspectiveToggleUI
{
    [SerializeField] ScalingUIElement doorUI, characterSelection, minimapUI;
    [SerializeField] GameObject stopButtonObject;
    [SerializeField] GameObject mainMenu, emojiUI, ingameUI;
    [SerializeField] bool skipMainMenuInEditor;

    Door currentDoor = null;
    UIState uiState;

    public event System.Action ReturnedToGame;

    public bool InLockedUIMode => uiState != UIState.InGame;

    private void Start()
    {
        GetLocalPlayer().PlayerStateChanged += OnPlayerStateChanged;

#if UNITY_EDITOR
        if (skipMainMenuInEditor)
            return;
#endif

        //EnterMainMenu();

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
                emojiUI.SetActive(uiState == UIState.InGame);
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
        uiState = UIState.InMainMenu;
        UpdateVisuals();
    }

    public void EnterMinimap()
    {
        uiState = UIState.InMinimap;
        UpdateVisuals();
    }

    public void EnterCharacterSelection()
    {
        uiState = UIState.InCharacterSelection;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        mainMenu.SetActive(uiState == UIState.InMainMenu);
        characterSelection.SetActiveTransition(uiState == UIState.InCharacterSelection);
        minimapUI.SetActiveTransition(uiState == UIState.InMinimap);
        ingameUI.SetActive(uiState == UIState.InGame);
        doorUI.SetActiveTransition(uiState == UIState.VisitCard);
        emojiUI.SetActive(uiState == UIState.InGame && GretaNetworkManager.Instance.IsConnected);
    }

    public void StartGame()
    {
        GretaNetworkManager.Instance?.GretaJoin();
        StartMenu.Instance.ExitStartMenu();
        ReturnToGame();
    }

    public void ReturnToGame()
    {
        uiState = UIState.InGame;
        ReturnedToGame?.Invoke();
        UpdateVisuals();
    }

    public void OpenDoor(Door d)
    {
        uiState = UIState.VisitCard;
        UpdateVisuals();
        currentDoor = d;
    }

    public void CloseDoor(Door d)
    {
        uiState = UIState.InGame;
        UpdateVisuals();
        currentDoor = null;
    }

    public void OpenCurrentDoorLink()
    {
        if (currentDoor != null)
        {
            currentDoor.OpenURL();
        }
    }

    public void CloseDoorFromMinimap()
    {
        uiState = UIState.InMinimap;
        UpdateVisuals();

        if (currentDoor != null)
        {
            currentDoor.CancelInteraction();
        }
        else
        {
            Debug.Log("Trying to close door, but door is null");
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

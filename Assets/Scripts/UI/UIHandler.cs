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
    [SerializeField] DoorUI doorUI;
    [SerializeField] ScalingUIElement characterSelection, minimapUI;
    [SerializeField] GameObject stopButtonObject;
    [SerializeField] GameObject mainMenu, emojiUI, ingameUI;
    [SerializeField] bool skipMainMenuInEditor;

    Door currentDoor = null;
    UIState uiState;
    UIState previousUiState;

    public event System.Action ReturnedToGame;

    public bool InLockedUIMode => UiState != UIState.InGame;

    public UIState UiState
    {
        get => uiState; set
        {
            previousUiState = uiState;
            uiState = value;
        }
    }

    private bool startedGame;

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

    private void OnConnectionStateChanged(GretaConnectionState state)
    {
        Debug.Log("Network State: " + state);
        switch (state)
        {
            case GretaConnectionState.Connected:
                emojiUI.SetActive(UiState == UIState.InGame);
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

    public void ReturnFromCharacterSelection()
    {
        UiState = UIState.InMainMenu;
        UpdateVisuals();
        StartMenu.Instance.JumpToMenuStage(1);
    }

    public void EnterMinimap()
    {
        UiState = UIState.InMinimap;
        UpdateVisuals();
    }

    public void EnterCharacterSelection()
    {
        UiState = UIState.InCharacterSelection;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        //mainMenu.SetActive(uiState == UIState.InMainMenu);
        characterSelection.SetActiveTransition(UiState == UIState.InCharacterSelection);
        minimapUI.SetActiveTransition(UiState == UIState.InMinimap);
        ingameUI.SetActive(UiState == UIState.InGame);
        doorUI.SetActiveTransition(UiState == UIState.VisitCard, currentDoor);
        emojiUI.SetActive(UiState == UIState.InGame && GretaNetworkManager.Instance.IsConnected);
    }

    public void StartGame()
    {
        startedGame = true;
        GretaNetworkManager.Instance?.GretaJoin();
        StartMenu.Instance.ExitStartMenu();
        ReturnToGame();
    }

    public void ReturnToGame()
    {
        if (startedGame)
        {
            UiState = UIState.InGame;
        }
        else
        {
            UiState = UIState.InMainMenu;
            StartMenu.Instance.JumpToMenuStage(1);
        }
        ReturnedToGame?.Invoke();

        UpdateVisuals();

    }

    public void OpenDoor(Door d)
    {
        UiState = UIState.VisitCard;
        currentDoor = d;
        UpdateVisuals();
    }

    public void CloseDoor(Door d)
    {
        UiState = UIState.InGame;
        currentDoor = null;
        UpdateVisuals();
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
        UiState = previousUiState;
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

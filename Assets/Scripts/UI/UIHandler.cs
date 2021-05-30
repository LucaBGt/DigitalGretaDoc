using Mirror;
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

public class UIHandler : MonoBehaviour, IPlayerUI, IPerspectiveToggleUI
{

    private LocalPlayerBehaviour GetLocalPlayer()
    {
        return LocalPlayerBehaviour.Instance;
    }

    public void StartTurnLeft()
    {
        Debug.Log("Start Turn Left");
        GetLocalPlayer().TurnLeft();
    }

    public void StartTurnRight()
    {
        Debug.Log("Start Turn Right");
        GetLocalPlayer().TurnRight();
    }
    public void EndTurnLeft()
    {
        Debug.Log("End Turn Left");
        GetLocalPlayer().StopTurning();
    }

    public void EndTurnRight()
    {
        Debug.Log("End Turn Right");
        GetLocalPlayer().StopTurning();
    }

    public void ToggleSwitchPerspective()
    {
        LocalPlayerBehaviour.Instance.TogglePerspective();
    }
}

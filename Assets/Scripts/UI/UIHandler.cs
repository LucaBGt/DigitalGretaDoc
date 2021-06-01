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
        GetLocalPlayer().TurnLeft();
    }

    public void StartTurnRight()
    {
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

    public void ToggleSwitchPerspective()
    {
        GetLocalPlayer().TogglePerspective();
    }
}

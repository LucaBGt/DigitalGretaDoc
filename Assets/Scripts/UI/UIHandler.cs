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

public class UIHandler : SingletonBehaviour<UIHandler>, IPlayerUI, IPerspectiveToggleUI
{
    [SerializeField] GameObject doorUI;

    Door currentDoor = null;

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
        if(currentDoor != null)
        {
            currentDoor.CancelInteraction();
        }
    }

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

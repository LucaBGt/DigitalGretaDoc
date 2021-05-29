using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFirstPersonTurnUI
{
    void StartTurnRight();
    void StartTurnLeft();
    void EndTurnRight();
    void EndTurnLeft();
}

public class UIHandler : MonoBehaviour, IFirstPersonTurnUI
{
    private PlayerBehaviour GetPlayerController2DTEMP()
    {
        foreach (PlayerBehaviour item in FindObjectsOfType<PlayerBehaviour>())
        {
            if (item.isLocalPlayer)
                return item;
        }

        return null;
    }
    public void StartTurnLeft()
    {
        Debug.Log("Start Turn Left");
        GetPlayerController2DTEMP().TurnLeft();
    }

    public void StartTurnRight()
    {
        Debug.Log("Start Turn Right");
        GetPlayerController2DTEMP().TurnRight();
    }
    public void EndTurnLeft()
    {
        Debug.Log("End Turn Left");
        GetPlayerController2DTEMP().StopTurning();
    }

    public void EndTurnRight()
    {
        Debug.Log("End Turn Right");
        GetPlayerController2DTEMP().StopTurning();
    }

}

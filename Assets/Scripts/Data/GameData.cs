using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public static class GameData
{
    public static changeControlScheme changeControlScheme = new changeControlScheme();
    public static setCanMove setCanMove = new setCanMove();
    public static setCanTurn setCanTurn = new setCanTurn();

    public static setCanTurn setLookMode = new setCanTurn();

    public static endHugWithMe _endHugWithMe = new endHugWithMe();
}
public class changeControlScheme : UnityEvent<controlScheme> { }
public class setCanMove : UnityEvent<bool> { }
public class setCanTurn : UnityEvent<bool> { }

public class endHugWithMe : UnityEvent<GameObject> { }



public enum controlScheme
{
    fps, mobile, maponly
}
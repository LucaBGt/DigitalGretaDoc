using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : SingletonBehaviour<GameInstance>
{

    private bool isQuitting;

    public static bool ApplicationQuitting => (Instance == null) ? false : Instance.isQuitting;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}

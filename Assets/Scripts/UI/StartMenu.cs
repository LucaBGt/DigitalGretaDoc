using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartMenu : MonoBehaviour
{
    [SerializeField] StartMenuStage[] startMenuStages;
    StartMenuStage current;

    private void OnEnable()
    {
        JumpToMenuStage(0);
    }

    public void JumpToMenuStage(int stage)
    {
        if (current != null)
            current.Exit();

        current = startMenuStages[stage];

        if (current != null)
            current.Enter();
    }
}

[System.Serializable]
public class StartMenuStage
{
    public CinemachineVirtualCamera virtualCamera;

    public void Enter()
    {
        virtualCamera.Priority = 20;
    }

    public void Exit()
    {
        virtualCamera.Priority = 0;
    }
}

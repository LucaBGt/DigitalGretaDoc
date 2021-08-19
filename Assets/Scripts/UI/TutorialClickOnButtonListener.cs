using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialClickOnButtonListener : ScalingUIElement
{
    [SerializeField] Button toListenTo;
    [SerializeField] ScalingUIElement followUp;


    void Start()
    {
        toListenTo.onClick.AddListener(() => FinishedTutorialStep());
    }

    public void FinishedTutorialStep()
    {
        toListenTo.onClick.RemoveAllListeners();

        if (followUp != null)
            followUp.SetActiveTransition(true);
        SetActiveTransition(false);
    }
}

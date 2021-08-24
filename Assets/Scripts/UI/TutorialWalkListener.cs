using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWalkListener : ScalingUIElement
{
    [SerializeField] ScalingUIElement followUp;


    void Start()
    {
        LocalPlayerBehaviour.Instance.onStartWalkingEvent.AddListener(() => FinishedTutorialStep());
    }

    public void FinishedTutorialStep()
    {
        LocalPlayerBehaviour.Instance.onStartWalkingEvent.RemoveAllListeners();
        if (followUp != null)
            followUp.SetActiveTransition(true);
        SetActiveTransition(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    string name { get; }
    void EnterInteraction();
    void ExitInteraction();

    Vector3 GetInteractPosition();

    float GetInteractYRotation();
}

public interface ICancallableInteractable : IInteractable
{
    event System.Action Cancel;
}


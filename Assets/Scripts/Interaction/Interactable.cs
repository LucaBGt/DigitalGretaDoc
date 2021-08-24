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

    Transform transform {get;}
}

public interface ICancallableInteractable : IInteractable
{
    event System.Action Cancel;
}


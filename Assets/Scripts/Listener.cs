using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listener : MonoBehaviour
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        GameData.changeControlScheme.AddListener(ChangeControlSchemeListener);
    }

    void ChangeControlSchemeListener(controlScheme cs)
    {
        Debug.Log("LISTENER: Control Scheme Changed");
    }

}

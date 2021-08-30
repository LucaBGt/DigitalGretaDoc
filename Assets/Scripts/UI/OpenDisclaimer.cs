using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDisclaimer : MonoBehaviour
{
    public void DoOpenDisclaimer()
    {
        //open the twitter app
        Application.OpenURL( "https://meine-greta.de/datenschutz-gretaland-app/" );
    }
}

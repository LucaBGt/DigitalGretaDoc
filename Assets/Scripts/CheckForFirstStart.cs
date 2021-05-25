using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckForFirstStart : MonoBehaviour
{
    // Start is called before the first frame update

    public bool debug = false;
    void Awake()
    {
        if (PlayerPrefs.GetString("firstStart", "false") == "true")
        {

        }
        else
        {
            if (debug)
                SceneManager.LoadScene(1);
        }
    }
}

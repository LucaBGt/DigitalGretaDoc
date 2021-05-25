using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{

    public List<Toggle> allSchemes;
    public ToggleGroup currentScheme;

    int nextScheme;

    controlScheme loadedScheme;

    public void Init()
    {
        allSchemes[PlayerPrefs.GetInt("currentControlScheme", 0)].isOn = true;
    }

    public void Close()
    {
        Debug.Log(nextScheme);
        loadedScheme = (controlScheme)nextScheme;
        GameData.changeControlScheme.Invoke(loadedScheme);
        GameData.setCanMove.Invoke(true);
        PlayerPrefs.SetInt("currentControlScheme", nextScheme);
        this.gameObject.SetActive(false);
    }

    public void changeScheme(int newScheme)
    {
        nextScheme = newScheme;
    }
}

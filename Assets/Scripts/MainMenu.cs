using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MainMenu : MonoBehaviour
{
    public GameObject _Visitor;
    public GameObject _Vendor;
    public GameObject _MainMenu;

    public NetworkManager network;
    public void Visitor()
    {
        _Visitor.SetActive(true);
        _MainMenu.SetActive(false);
    }
    public void Vendor()
    {
        _Vendor.SetActive(true);
        _MainMenu.SetActive(false);
    }
    public void BackToMain()
    {
        _MainMenu.SetActive(true);
        _Vendor.SetActive(false);
        _Visitor.SetActive(false);
    }

    public void Join(bool _Vendor)
    {
        string ipAdress = "162.0.225.63";
        ipAdress = "localhost";

        CharacterMessage characterMessage = new CharacterMessage();

        if (_Vendor)
        {
            characterMessage.characterType = 1;
        }
        else
        {
            characterMessage.characterType = 0;
        }

        //network.message = characterMessage;

        network.networkAddress = ipAdress;
        network.StartClient();
    }

}
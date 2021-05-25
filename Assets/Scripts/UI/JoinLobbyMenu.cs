using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        //NetworkManager.OnClientConnected += HandleClientConnected;
        //NetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}

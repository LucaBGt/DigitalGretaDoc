using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ConnectionMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();
        landingPagePanel.SetActive(false);
    }
}

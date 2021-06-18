using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum GretaConnectionState
{
    Disconnected,
    Connected
}

public class GretaNetworkManager : NetworkManager
{
    private static GretaNetworkManager instance;


    public bool IsConnected => NetworkClient.isConnected;

    public event System.Action<GretaConnectionState> ConnectionStateChanged;

    public static GretaNetworkManager Instance => instance;

    public override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            base.Awake();
        }
        else
        {
            Debug.LogError($"Spawned Second Instance of {GetType()}, destroying");
            Destroy(gameObject);
        }
    }


    public override void Start()
    {
        //override default bool
        #if UNITY_SERVER
        Application.targetFrameRate = 50;
        StartServer();
        #endif
    }

    public void GretaJoin()
    {
        StartClient();
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        //in case of testing with local host
        ConnectionStateChanged?.Invoke(GretaConnectionState.Disconnected);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        //Connected 
        base.OnClientConnect(conn);
        ConnectionStateChanged?.Invoke(GretaConnectionState.Connected);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        //Failed to connect
        Debug.Log("Disconnected / Connection Failed");
        ConnectionStateChanged?.Invoke(GretaConnectionState.Disconnected);

        //Fallback??? TODO
    }
}

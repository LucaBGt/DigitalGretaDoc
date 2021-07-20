using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public enum GretaConnectionState
{
    Disconnected,
    Connected,
    AttemptingReconnectInDelay,
    AttemptConnection,
    OfflineMode
}

public class GretaNetworkManager : NetworkManager
{
    private static GretaNetworkManager instance;

    [SerializeField] float reconnectDelay;

    private int connectionAttemptCounter = 0;
    private InternalState internalState = InternalState.Initializing;

    public event System.Action<GretaConnectionState> ConnectionStateChanged;

    public bool IsConnected => NetworkClient.isConnected;
    public float ReconnectDelay => reconnectDelay;
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
            Debug.LogError($"Spawned Second Instance of GretaNetworkManager, destroying");
            Destroy(gameObject);
        }
    }


    public override void Start()
    {
        //override default bool
#if UNITY_SERVER
        Application.targetFrameRate = 50;
        StartServer();
#else

        ConnectionStateChanged += OnConnectionStateChanged;
        internalState = InternalState.Ready;
#endif

    }

    private void OnApplicationPause(bool pause)
    {
        if (internalState != InternalState.Active) return;

        if (pause)
        {
            StopClient();
            StopAllCoroutines();
        }
        else
        {
            connectionAttemptCounter = 0;
            GretaJoin();
        }
    }

    private void OnConnectionStateChanged(GretaConnectionState state)
    {
        if (state == GretaConnectionState.Disconnected)
        {
            if (connectionAttemptCounter >= 3)
            {
                StartCoroutine(OfflineModeRoutine());
            }
            else
            {
                StartCoroutine(DisconnectedRoutine());
            }
        }
    }

    private IEnumerator OfflineModeRoutine()
    {
        yield return new WaitForSeconds(1);
        ConnectionStateChanged?.Invoke(GretaConnectionState.OfflineMode);
    }

    private IEnumerator DisconnectedRoutine()
    {
        yield return new WaitForSeconds(1);
        ConnectionStateChanged?.Invoke(GretaConnectionState.AttemptingReconnectInDelay);
        yield return new WaitForSeconds(reconnectDelay);
        GretaJoin();
    }

    public void GretaJoin()
    {
        connectionAttemptCounter++;
        ConnectionStateChanged?.Invoke(GretaConnectionState.AttemptConnection);
        StartClient();
        internalState = InternalState.Active;
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        //in case of testing with local host
        //ConnectionStateChanged?.Invoke(GretaConnectionState.Disconnected);
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

        Debug.Log("Disconnected / Connection Failed");
        ConnectionStateChanged?.Invoke(GretaConnectionState.Disconnected);
    }

    public enum InternalState
    {
        Initializing,
        Ready,
        Active
    }
}

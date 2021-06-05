using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GretaNetworkManager : NetworkManager
{
    private static GretaNetworkManager instance;

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
               StartServer();
        #endif
    }

    public void GretaJoin()
    {
        StartClient();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        //Connected 
        Debug.Log("Connected " + conn);
        base.OnClientConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        //Failed to connect
        Debug.Log("Disconnected / Connection Failed: " + conn);

        //Fallback??? TODO
    }
}

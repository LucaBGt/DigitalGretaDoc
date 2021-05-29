using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GretaNetworkManager : NetworkManager
{
    [Header("Greta")]
    [SerializeField] bool editorStartsHost;

    public override void Start()
    {

#if UNITY_SERVER
               StartServer();
#else


#if UNITY_EDITOR
        if (editorStartsHost)
        {
            StartHost();
        }
        else
        {
            StartClient();
        }

#else
        StartClient();
#endif
#endif

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

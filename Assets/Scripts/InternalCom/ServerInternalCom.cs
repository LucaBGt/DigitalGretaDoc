#define TEST_INTERNAL_COM

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Unity.Profiling;
using UnityEngine;


public class ServerInternalCom : MonoBehaviour
{
    [SerializeField] ushort port;

    readonly ConcurrentDictionary<int, ServerInternalComConnection> clients = new ConcurrentDictionary<int, ServerInternalComConnection>();

    TcpListener tcpListener;
    Thread listenerThread;

    int connectionIDCounter;
    bool setup = false;

    private void Awake()
    {
#if UNITY_SERVER || TEST_INTERNAL_COM
        Setup();
#else
        Destroy(this);
#endif
    }

    private void Setup()
    {
        setup = true;
        Debug.Log("Setup Internal Com");
        listenerThread = new Thread(() => { ListenerThreadBehaviour(port); });
        listenerThread.IsBackground = true;
        listenerThread.Priority = System.Threading.ThreadPriority.BelowNormal;
        listenerThread.Start();
        Application.logMessageReceived += OnLogMessageRecieved;
    }

    private void OnLogMessageRecieved(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Exception:
                SendLog($"Ex: {condition} \n {stackTrace}");
                break;

            case LogType.Error:
                SendLog($"Error: {condition} \n {stackTrace}");
                break;

        }
    }

    private void FixedUpdate()
    {
        //Debug.Log($"Connections: {clients.Count}");
        List<int> toRemove = new List<int>();

        foreach (var item in clients)
        {
            if (item.Value.Connected)
            {
                item.Value.Update();
            }
            else
            {
                toRemove.Add(item.Key);
            }
        }

        foreach (var val in toRemove)
        {
            clients.TryRemove(val, out ServerInternalComConnection c);
        }
    }

    [NaughtyAttributes.Button]
    private void SendTest()
    {
        Send(InternalComPacket.MakeLog("Test"));
    }

    [NaughtyAttributes.Button]
    private void SendExceptionTest()
    {
        throw new System.Exception("Intentional Exception");
    }

    public void Send(byte[] data)
    {
        foreach (var client in clients)
        {
            client.Value.Send(data);
        }
    }

    public void SendLog(string msg)
    {
        Send(InternalComPacket.MakeLog(msg));
    }

    private void ListenerThreadBehaviour(int port)
    {
        tcpListener = new TcpListener(System.Net.IPAddress.Loopback, port);
        tcpListener.Start();
        Debug.Log("InternalCom: listening on port=" + port);

        while (true)
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();

            int connId = NextConnectionId();
            clients[connId] = new ServerInternalComConnection(tcpClient);
        }
    }



    private void OnDestroy()
    {
        if (!setup) return;

        Application.logMessageReceived -= OnLogMessageRecieved;
        Debug.Log("Shutting Down Internal Com");
        SendLog("Shutting Down");
        listenerThread?.Abort();
        tcpListener?.Stop();

        foreach (var idClientPair in clients)
        {
            idClientPair.Value.Dispose();
        }
    }

    private int NextConnectionId()
    {
        return Interlocked.Increment(ref connectionIDCounter);
    }
}

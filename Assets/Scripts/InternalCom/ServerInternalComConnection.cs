using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ServerInternalComConnection : IDisposable
{
    const float STAY_ALIVE_RESEND_TIME = 30;
    TcpClient client;
    Thread recieveThread;
    private bool closed;
    private float lastPacketStamp;

    readonly ConcurrentBag<byte[]> unhandledSegments = new ConcurrentBag<byte[]>();

    public bool Connected => !closed;

    public ServerInternalComConnection(TcpClient client)
    {
        Debug.Log($"Started InternalComConnection with {client.Client.RemoteEndPoint}");
        this.client = client;

        recieveThread = new Thread(RecieveThreadBehaviour);
        recieveThread.IsBackground = true;
        recieveThread.Start();
    }

    public void Update()
    {
        if (!client.Connected) return;
        
        HandleSegments();

        if(Time.time - lastPacketStamp > STAY_ALIVE_RESEND_TIME)
        {
            Send(InternalComPacket.MakeStayAlive());
        }
    }

    private void HandleSegments()
    {
        if (unhandledSegments.TryTake(out byte[] data))
        {
            lastPacketStamp = Time.time;
            InternalComPacket.Type type = InternalComPacket.GetType(data, data.Length);

            Debug.Log($"Handling {type}: {BitConverter.ToString(data)}");

            switch (type)
            {
                case InternalComPacket.Type.Ping:
                    Send(InternalComPacket.MakePong());
                    break;
            }
        }
    }

    public void Send(byte[] data)
    {
        lastPacketStamp = Time.time;
        //catch closed exceptions
        //use cancellationtoken?
        client.GetStream().WriteAsync(data, 0, data.Length);
    }

    private void RecieveThreadBehaviour()
    {
        var stream = client.GetStream();
        byte[] dataBuffer = new byte[256];

        while (true)
        {
            try
            {
                int read = stream.Read(dataBuffer, 0, dataBuffer.Length);

                if (read > 0)
                {
                    byte[] data = new byte[read];
                    Array.Copy(dataBuffer, data, read);
                    unhandledSegments.Add(data);
                }
            }
            catch (IOException)
            {
                //IO Exception in TCPClient is fired when the connection is closed
                closed = true;
            }
        }
    }

    public void Dispose()
    {
        if (recieveThread != null)
            recieveThread.Abort();
        client.Close();
    }
}


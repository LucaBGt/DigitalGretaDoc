using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InternalComPacket
{
    public enum Type
    {
        Error = 0,
        StayAlive = 99,
        Ping = 100,
        Pong,
        Log,
    }

    public static byte[] MakeLog(string msg)
    {
        byte[] text = System.Text.Encoding.UTF8.GetBytes(msg);
        byte[] output = new byte[text.Length + 1];
        output[0] = (byte)Type.Log;
        Array.Copy(text, 0, output, 1, text.Length);
        return output;
    }

    public static byte[] MakePing()
    {
        return new byte[1] { (byte)Type.Ping };
    }

    public static byte[] MakeStayAlive()
    {
        return new byte[1] { (byte)Type.StayAlive };
    }

    public static byte[] MakePong()
    {
        return new byte[1] { (byte)Type.Pong };
    }

    public static string ToString(byte[] packetBuffer, int length)
    {
        return GetType(packetBuffer, length).ToString();
    }

    public static Type GetType(byte[] packetBuffer, int length)
    {
        if (length == 0) return Type.Error;
        Type t = (Type)packetBuffer[0];
        return t;
    }

    
}

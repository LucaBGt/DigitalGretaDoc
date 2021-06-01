using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GretaNetworkProfiler : MonoBehaviour
{
    [SerializeField] bool runDiagnostic;
    [SerializeField] bool logExtensively;

    [SerializeField] int secondsTracked;
    [SerializeField] Rect outRect, inRect;

    ProfiledSecond[] outSeconds;
    ProfiledSecond[] inSeconds;
    int outMax = 100;
    int inMax = 100;
    float oldTimeStamp;

    Texture2D boxTex;

    private void Awake()
    {
        if (runDiagnostic)
        {
            outSeconds = new ProfiledSecond[secondsTracked];
            inSeconds = new ProfiledSecond[secondsTracked];
            SetupTexture(ref boxTex, Color.white);
        }
        else
        {
            Destroy(this);
        }
    }

    private void SetupTexture(ref Texture2D tex, Color c)
    {
        tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, c);
        tex.Apply();
    }

    private void OnEnable()
    {
        if (runDiagnostic)
        {
            NetworkDiagnostics.OutMessageEvent += OnSendMessage;
            NetworkDiagnostics.InMessageEvent += OnRecieveMessage;
        }
    }

    private void OnDisable()
    {
        NetworkDiagnostics.OutMessageEvent -= OnSendMessage;
        NetworkDiagnostics.InMessageEvent -= OnRecieveMessage;
    }

    private void Update()
    {
        if (!runDiagnostic) return;

        if (Mathf.FloorToInt(oldTimeStamp) != Mathf.FloorToInt(Time.time))
            ClearCurrentSecond();

        oldTimeStamp = Time.time;
    }

    private void ClearCurrentSecond()
    {
        outSeconds[GetCurrentIndex()].Clear();
        inSeconds[GetCurrentIndex()].Clear();
    }

    private void OnRecieveMessage(NetworkDiagnostics.MessageInfo obj)
    {
        if (!runDiagnostic) return;

        if (logExtensively)
            Debug.Log($"Recv {obj.bytes}b: {obj.message}");
        inSeconds[GetCurrentIndex()].Bytes += obj.bytes;

        int bytes = GetBytesRecievedThisSecond();
        if (bytes > inMax)
            inMax = bytes;
    }

    private void OnSendMessage(NetworkDiagnostics.MessageInfo obj)
    {
        if (!runDiagnostic) return;

        if (logExtensively)
            Debug.Log($"Sent {obj.bytes}b: {obj.message}");
        outSeconds[GetCurrentIndex()].Bytes += obj.bytes;

        int bytes = GetBytesSentThisSecond();
        if (bytes > outMax)
            outMax = bytes;
    }

    private int GetBytesSentThisSecond()
    {
        return outSeconds[GetCurrentIndex()].Bytes;
    }

    private int GetBytesRecievedThisSecond()
    {
        return inSeconds[GetCurrentIndex()].Bytes;
    }

    private int GetCurrentIndex()
    {
        return Mathf.FloorToInt(Time.time) % secondsTracked;
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(outRect.x, outRect.y - outRect.height - 25, 100, 25), "OUT");
        GUIDrawBarChart(outSeconds, outMax, outRect);
        GUI.Label(new Rect(inRect.x, inRect.y - inRect.height - 25, 100, 25), "IN");
        GUIDrawBarChart(inSeconds, inMax, inRect);
    }

    private void GUIDrawBarChart(ProfiledSecond[] arr, int max, Rect rect)
    {
        GUI.color = Color.black;
        float width = rect.width / secondsTracked;
        float height = rect.height;
        GUI.Label(new Rect(rect.x - 25, rect.y - height - 13, 25, 25), max.ToString());
        GUI.Label(new Rect(rect.x - 25, rect.y - height * 0.5f - 13, 25, 25), (max / 2).ToString());
        GUI.Label(new Rect(rect.x - 25, rect.y - 13, 25, 25), "0");


        for (int i = 0; i < secondsTracked; i++)
        {
            ProfiledSecond ps = arr[i];
            float h = height * ((float)ps.Bytes / max);
            float y = rect.y - h;
            Rect r = new Rect(rect.x + width * i, y, width, h);
            GUI.color = GetCurrentIndex() == i ? Color.blue : Color.black;
            GUI.DrawTexture(r, boxTex);
            GUI.color = Color.black;
        }
    }

    public struct ProfiledSecond
    {
        public int Bytes;

        public void Clear()
        {
            Bytes = 0;
        }
    }
}


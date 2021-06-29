using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINetworkConnectionVisualizer : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI mainText, subText;
    [SerializeField] GameObject button;

    public void Reconnect()
    {
        GretaNetworkManager.Instance?.GretaJoin();
    }

    private void Start()
    {
        GretaNetworkManager.Instance.ConnectionStateChanged += OnConnectionStateChanged;
        ResetText();
    }

    private void OnDestroy()
    {
        if (GretaNetworkManager.Instance != null)
            GretaNetworkManager.Instance.ConnectionStateChanged -= OnConnectionStateChanged;
    }

    private void OnConnectionStateChanged(GretaConnectionState state)
    {
        StopAllCoroutines();
        switch (state)
        {
            case GretaConnectionState.Connected:
                StartCoroutine(ConnectedRoutine());
                break;

            case GretaConnectionState.Disconnected:
                SetText("Verbindung Fehlgeschlagen", "");
                break;

            case GretaConnectionState.AttemptingReconnectInDelay:
                StartCoroutine(AttemptingReconnectRoutine());
                break;

            case GretaConnectionState.AttemptConnection:
                SetText("Verbindung...", "");
                break;


            case GretaConnectionState.OfflineMode:
                SetText("Offline Modus", "", buttonOn: true);
                break;
        }
    }

    private IEnumerator ConnectedRoutine()
    {
        SetText("Verbindung hergestellt", "");
        yield return new WaitForSeconds(2.5);
        ResetText();
    }

    private IEnumerator AttemptingReconnectRoutine()
    {
        float t = GretaNetworkManager.Instance.ReconnectDelay;

        while (t > 0)
        {
            SetText("Verbindung Fehlgeschlagen", $"Nächster versuch in {t.ToString("0")}");
            yield return null;
            t -= Time.deltaTime;
        }
    }


    private void SetText(string main, string sub, bool buttonOn = false)
    {
        mainText.text = main;
        subText.text = sub;
        button.SetActive(buttonOn);
    }

    private void ResetText()
    {
        SetText("", "");
    }

}

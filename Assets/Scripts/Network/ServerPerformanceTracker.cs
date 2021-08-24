using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;

public class ServerPerformanceTracker : MonoBehaviour
{
#if UNITY_SERVER
    const string check_file = "server_settings.json";

    private ServerSettings settings;

    public void Start()
    {
        UnityEngine.Debug.Log("Starting Server Performance Tracker, loading ");
        ReloadSettings();
       

        if(settings.LogPerformance)
        {
            UnityEngine.Debug.Log("LogPerormance enabled, starting to log");

            if(!Directory.Exists(Application.dataPath + "/Log"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Log");
            }

            StartCoroutine(LogRoutine());
        }
    }

    private void ReloadSettings()
    {
        string path = Application.dataPath + "/" + check_file;
        if(File.Exists(path))
        {
            string settingsText = File.ReadAllText(path);
            settings = JsonUtility.FromJson<ServerSettings>(settingsText);            
        }
        else
        {
            settings = new ServerSettings();
            File.WriteAllText(path, JsonUtility.ToJson(settings));
            UnityEngine.Debug.Log("server_settings not found. Generating at " + path);
        }
    }

    private IEnumerator LogRoutine()
    {
        System.Diagnostics.PerformanceCounter cpuCounter;
        System.Diagnostics.PerformanceCounter  ramCounter;

        cpuCounter = new System.Diagnostics.PerformanceCounter ("Processor", "% Processor Time", "_Total");
        ramCounter = new System.Diagnostics.PerformanceCounter ("Memory", "Available MBytes");  

        while(settings.LogPerformance)
        {
            AppendStatusLog(cpuCounter.NextValue() + ", " + ramCounter.NextValue() );
            yield return new WaitForSeconds(10);
            ReloadSettings();
        }
    }

    private void AppendStatusLog(string additional)
    {
        int users = GretaNetworkManager.Instance.numPlayers;

        string message  = DateTime.Now.ToString("HH.mm.ss") + ", " + users + ", " + additional;
        var writer = File.AppendText(Application.dataPath + "/Log/" + DateTime.Now.ToString("yyyy_MM_dd") + ".log");
        writer.WriteLine(message);
        writer.Close();
        //UnityEngine.Debug.Log(message);
    }

    [System.Serializable]
    public class ServerSettings
    {
        public bool LogPerformance;
    }


#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

[DefaultExecutionOrder(-100)]
public class DownloadManager : SingletonBehaviour<DownloadManager>
{
    const string URL_VENDOR_GET = "get";
    public static string ROOT_PATH;

    [SerializeField]
    private string urlVendorRequests = "http://82.165.109.5:8082/";

    private int downloadingCount = 0;

    public string UrlVendorRequests => urlVendorRequests;

    private void Start()
    {
        ROOT_PATH = Application.persistentDataPath + "/data/";
        Debug.Log($"Download Manager started. Data path: {ROOT_PATH}");

        if (!Directory.Exists(ROOT_PATH))
        {
            Debug.Log("No persistent data directory found, creating.");
            Directory.CreateDirectory(ROOT_PATH);
        }

        string[] files = Directory.GetFiles(ROOT_PATH);

    }


    public string GetFileAsString(string name)
    {
        Debug.Log("Trying to load: " + ROOT_PATH + name);
        if (File.Exists(ROOT_PATH + name))
        {
            return File.ReadAllText(ROOT_PATH + name);
        }
        return null;
    }

    public void SaveStringToFile(string filename, string content)
    {
        File.WriteAllText(ROOT_PATH + filename, content);
    }

    public TextureRequest GetTexture(string directory, string name)
    {
        string localPath = $"{ROOT_PATH}{directory}_{name}";
        if (File.Exists(localPath))
        {
            Debug.Log($"Request {directory} {name} found locally.");
            byte[] data = File.ReadAllBytes(localPath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return TextureRequest.MakeReadyRequest(directory, name, tex);
        }
        else
        {
            Debug.Log($"Request {directory} {name} not found, initiating download.");
            TextureRequest req = TextureRequest.MakeDownloadRequest(directory, name, urlVendorRequests + URL_VENDOR_GET, this);
            req.FinishedDownload += OnDownloadReady;
            ++downloadingCount;
            return req;
        }
    }

    private void OnDownloadReady(TextureRequest obj)
    {
        if (string.IsNullOrEmpty(obj.Error))
        {
            string localPath = $"{ROOT_PATH}{obj.Directory}_{obj.Name}";
            File.WriteAllBytes(localPath, obj.TextureData);
            obj.DisposeOfTextureData();
            Debug.Log($"DownloadManager: downloaded and saved {obj.Directory}/{obj.Name}");
        }
        else
        {
            Debug.LogWarning($"DownloadManager: Failed to download {obj.Directory}/{obj.Name}: {obj.Error}");
        }

        --downloadingCount;
        obj.FinishedDownload -= OnDownloadReady;
    }
}

public class TextureRequest
{
    private Texture2D texture;
    private byte[] textureData = null;

    private bool ready;
    private string error;

    private string directory;
    private string name;

    public event System.Action<TextureRequest> FinishedDownload;

    public string Error => error;
    public string Directory => directory;
    public string Name => name;
    public bool IsReady => ready;
    public Texture2D Texture => ready ? texture : null;
    public byte[] TextureData => textureData;


    public static TextureRequest MakeReadyRequest(string directory, string name, Texture2D tex)
    {
        TextureRequest req = new TextureRequest();
        req.ready = true;
        req.texture = tex;
        req.directory = directory;
        req.name = name;
        return req;
    }

    public static TextureRequest MakeDownloadRequest(string directory, string name, string baseUrl, MonoBehaviour routineRunner)
    {
        TextureRequest req = new TextureRequest();
        req.directory = directory;
        req.name = name;
        req.texture = new Texture2D(2, 2);
        string path = $"{baseUrl}/{directory}_{name}";
        routineRunner.StartCoroutine(req.DownloadTextureRoutine(path));
        return req;
    }

    public void DisposeOfTextureData()
    {
        textureData = null;
    }

    private IEnumerator DownloadTextureRoutine(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(path))
        {
            //Is this too high? What do we do in case of download failed?.
            uwr.timeout = 600;

            yield return uwr.SendWebRequest();
            if (string.IsNullOrEmpty(uwr.error))
            {
                textureData = uwr.downloadHandler.data;
                texture.LoadImage(textureData);
            }
            else
            {
                error = uwr.error;
            }

            ready = true;
            FinishedDownload?.Invoke(this);
        }
    }
}

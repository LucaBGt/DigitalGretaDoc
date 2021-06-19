using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class VendorsHander : MonoBehaviour
{
    const string URL_VENDOR_HASH = "http://82.165.109.5:8082/vendor_hash";
    const string URL_VENDOR_DATA = "http://82.165.109.5:8082/vendor_data";

    private string vendorDataPath;
    private string vendorJSONPath;

    private Dictionary<int, RuntimeVendorData> vendorDataCache = new Dictionary<int, RuntimeVendorData>();

    GretaMarketVendorPackage vendorInfo;

    private void Awake()
    {
#if UNITY_SERVER
        Destroy(this);
#else
        Setup();
#endif
    }

    private void Setup()
    {
        vendorDataPath = Application.persistentDataPath + "/Vendors/";
        vendorJSONPath = vendorDataPath + "data.json";


        StartCoroutine(SetupRoutine());
    }

    private IEnumerator SetupRoutine()
    {
        yield return StartCoroutine(LoadJSON());

        yield return StartCoroutine(CheckHashAndQueueDownload());
    }

    private IEnumerator LoadJSON()
    {
        //use filesystem for loading/deleting, avoid in WEBGL completely
        vendorDataCache = new Dictionary<int, RuntimeVendorData>();
        UnityWebRequest request = UnityWebRequest.Get("file//" + vendorJSONPath);
        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError($"Error loading {vendorJSONPath}: {request.error}");
                yield break;
            case UnityWebRequest.Result.Success:
                vendorInfo = JsonUtility.FromJson<GretaMarketVendorPackage>(request.downloadHandler.text);
                Debug.Log($"Loaded vendorInfo with hash: {vendorInfo.Hash}");
                break;
        }
    }

    private IEnumerator CheckHashAndQueueDownload()
    {
        var request = UnityWebRequest.Get(URL_VENDOR_HASH);

        yield return request.SendWebRequest();

        string hash = null;

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(URL_VENDOR_HASH + ": Error: " + request.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(URL_VENDOR_HASH + ": HTTP Error: " + request.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(URL_VENDOR_HASH + ":\nReceived: " + request.downloadHandler.text);
                hash = request.downloadHandler.text;
                break;
        }

        if (string.IsNullOrEmpty(hash) || (vendorInfo != null && vendorInfo.Hash == hash))
        {
            Debug.Log("Cancelling VendorData download.");
        }
        else
        {
            yield return StartCoroutine(DownloadDataRoutine());
        }
    }

    private IEnumerator DownloadDataRoutine()
    {
        var request = UnityWebRequest.Get(URL_VENDOR_DATA);

        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(URL_VENDOR_DATA + ": Error: " + request.error);
                yield break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(URL_VENDOR_DATA + ": HTTP Error: " + request.error);
                yield break;
            case UnityWebRequest.Result.Success:
                Debug.Log(URL_VENDOR_DATA + ":\n Completed ");
                break;
        }

        //??? What do I do now?!
        //Switch to downloading raw textures

    }


    public RuntimeVendorData GetRuntimeVendorData(int id)
    {
        if (vendorInfo == null)
        {
            Debug.LogError("Trying to access VendorData, but vendorInfo is not loaded.");
            return null;
        }

        if (vendorDataCache.ContainsKey(id))
        {
            return vendorDataCache[id];
        }
        else
        {
            if (id < 0 || id >= vendorInfo.Vendors.Length)
            {
                Debug.LogError($"Trying to access vendorInfo out of range: {id}");
                return null;
            }

            RuntimeVendorData data = new RuntimeVendorData(vendorInfo.Vendors[id], vendorDataPath, this);
            vendorDataCache.Add(id, data);
            return data;
        }
    }

}

public class RuntimeVendorData
{
    private VendorData data;

    Texture2D logoTexture;
    Texture2D mainImageTexture;
    Texture2D[] subImagesTextures;

    private int loadedTextures = 0;

    public RuntimeVendorData(VendorData _data, string rootPath, MonoBehaviour coroutineRunner)
    {
        data = _data;
        Debug.Log($"Started loading vendor {_data.Name}");
        StartLoadTextures(rootPath, coroutineRunner);
    }

    private void StartLoadTextures(string rootPath, MonoBehaviour coroutineRunner)
    {
        logoTexture = new Texture2D(2, 2);
        coroutineRunner.StartCoroutine(LoadTexture(rootPath + data.LogoFile, logoTexture));

        mainImageTexture = new Texture2D(2, 2);
        coroutineRunner.StartCoroutine(LoadTexture(rootPath + data.LogoFile, logoTexture));

        subImagesTextures = new Texture2D[data.SubImagesFiles.Length];

        for (int i = 0; i < subImagesTextures.Length; i++)
        {
            subImagesTextures[i] = new Texture2D(2, 2);
            coroutineRunner.StartCoroutine(LoadTexture(rootPath + data.SubImagesFiles[i], subImagesTextures[i]));
        }
    }

    private IEnumerator LoadTexture(string path, Texture2D tex)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(path))
        {
            yield return uwr.SendWebRequest();
            if (string.IsNullOrEmpty(uwr.error))
            {
                tex.LoadImage(uwr.downloadHandler.data);
            }
            else
            {
                Debug.LogError("LoadTextureError: " + uwr.error);
            }
            loadedTextures++;
        }
    }

    public bool IsFullyLoaded()
    {
        return loadedTextures == subImagesTextures.Length + 2;
    }
}

[System.Serializable]
public class GretaMarketVendorPackage
{
    public string Hash;

    public VendorData[] Vendors;
}

[System.Serializable]
public class VendorData
{
    public string Name;
    public string Description;
    public string LinkWebsite;
    public string LinkFacebook;
    public string LinkInstagram;
    public string LinkZoom;

    public string LogoFile;
    public string MainImageFile;
    public string[] SubImagesFiles;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class VendorsHander : SingletonBehaviour<VendorsHander>
{
    [SerializeField]
    private string urlVendorRequests = "http://82.165.109.5:8082/";

    const string URL_VENDOR_HASH = "vendor_hash";
    const string URL_VENDOR_JSON = "vendor_data";
    const string URL_VENDOR_GET = "get";

    private string vendorDataPathInternal;
    private string vendorJSONPathInternal;

    private Dictionary<int, RuntimeVendorData> vendorDataCache = new Dictionary<int, RuntimeVendorData>();

    GretaMarketVendorPackage vendorInfo;

    public event System.Action Ready;

    public int VendorsCount => vendorDataCache.Count;

    private void Start()
    {
#if UNITY_SERVER
        Destroy(this);
#else
        Setup();
#endif
    }

    private void Setup()
    {

        StartCoroutine(SetupRoutine());
    }

    private IEnumerator SetupRoutine()
    {
        //handle loading internal data later, for now always download
        //yield return StartCoroutine(LoadCachedData());

        yield return StartCoroutine(CheckHashAndQueueDownload());
    }

    private IEnumerator LoadCachedData()
    {
        //use filesystem for loading/deleting, avoid in WEBGL completely
        vendorDataCache = new Dictionary<int, RuntimeVendorData>();


        UnityWebRequest request = UnityWebRequest.Get("file//" + vendorJSONPathInternal);
        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError($"Error loading {vendorJSONPathInternal}: {request.error}");
                yield break;
            case UnityWebRequest.Result.Success:
                vendorInfo = JsonUtility.FromJson<GretaMarketVendorPackage>(request.downloadHandler.text);
                Debug.Log($"Loaded vendorInfo with hash: {vendorInfo.Hash}");
                break;
        }
    }

    private IEnumerator CheckHashAndQueueDownload()
    {
        var request = UnityWebRequest.Get(urlVendorRequests + URL_VENDOR_HASH);

        yield return request.SendWebRequest();

        string hash = null;

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogWarning(URL_VENDOR_HASH + ": Error: " + request.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogWarning(URL_VENDOR_HASH + ": HTTP Error: " + request.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(URL_VENDOR_HASH + ":\nReceived: " + request.downloadHandler.text);
                hash = request.downloadHandler.text;
                break;
        }

        if (string.IsNullOrEmpty(hash) || (vendorInfo != null && vendorInfo.Hash == hash))
        {
            Debug.Log("Cancelling VendorData download.");
            Ready?.Invoke();
        }
        else
        {
            yield return StartCoroutine(DownloadDataRoutine());
        }
    }

    private IEnumerator DownloadDataRoutine()
    {
        var request = UnityWebRequest.Get(urlVendorRequests + URL_VENDOR_JSON);

        yield return request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(URL_VENDOR_JSON + ": Error: " + request.error);
                yield break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(URL_VENDOR_JSON + ": HTTP Error: " + request.error);
                yield break;
            case UnityWebRequest.Result.Success:
                Debug.Log(URL_VENDOR_JSON + ":\n Completed ");
                break;
        }

        vendorInfo = JsonUtility.FromJson<GretaMarketVendorPackage>(request.downloadHandler.text);

        if (vendorInfo == null)
        {
            Debug.LogError("Failed to create JSON from download.");
            yield break;
        }

        vendorDataCache = new Dictionary<int, RuntimeVendorData>();

        int i = 0;
        foreach (var vendor in vendorInfo.Vendors)
        {
            var data = new RuntimeVendorData(vendor, urlVendorRequests + URL_VENDOR_GET, this);
            vendorDataCache.Add(i++, data);
        }

        Ready?.Invoke();
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

            RuntimeVendorData data = new RuntimeVendorData(vendorInfo.Vendors[id], vendorDataPathInternal, this);
            vendorDataCache.Add(id, data);
            return data;
        }
    }

}

public class RuntimeVendorData
{
    private VendorData data;

    public Texture2D LogoTexture;
    public Texture2D MainImageTexture;
    public Texture2D[] SubImagesTextures;

    private int loadedTextures = 0;

    public VendorData InternalData => data;

    public RuntimeVendorData(VendorData _data, string rootPath, MonoBehaviour coroutineRunner)
    {
        data = _data;
        Debug.Log($"Started loading vendor {_data.Name}");
        StartLoadTextures(rootPath + "/" + data.Directory + "_", coroutineRunner);
    }

    private void StartLoadTextures(string retrievePath, MonoBehaviour coroutineRunner)
    {
        LogoTexture = new Texture2D(2, 2);
        coroutineRunner.StartCoroutine(LoadTexture(retrievePath + data.LogoFile, LogoTexture));

        MainImageTexture = new Texture2D(2, 2);
        coroutineRunner.StartCoroutine(LoadTexture(retrievePath + data.MainImageFile, MainImageTexture));

        SubImagesTextures = new Texture2D[data.SubImagesFiles.Length];

        for (int i = 0; i < SubImagesTextures.Length; i++)
        {
            SubImagesTextures[i] = new Texture2D(2, 2);
            coroutineRunner.StartCoroutine(LoadTexture(retrievePath + data.SubImagesFiles[i], SubImagesTextures[i]));
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
                Debug.Log($"Loaded texture {path}");
            }
            else
            {
                Debug.LogError($"LoadTextureError: {path}  {uwr.error}" );
            }
            loadedTextures++;
        }
    }

    public bool IsFullyLoaded()
    {
        return loadedTextures == SubImagesTextures.Length + 2;
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

    public string Directory;
}

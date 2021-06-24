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
    const string LOCAL_DATA_NAME = "data.json";

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
        LoadCachedData();
        yield return StartCoroutine(CheckHashAndSetupData());
    }

    private void LoadCachedData()
    {
        string data = DownloadManager.Instance.GetFileAsString(LOCAL_DATA_NAME);

        if (!string.IsNullOrEmpty(data))
        {
            vendorInfo = JsonUtility.FromJson<GretaMarketVendorPackage>(data);
        }else
        {
            Debug.Log("No local data.json found.");
        }
    }

    private IEnumerator CheckHashAndSetupData()
    {
        var request = UnityWebRequest.Get(urlVendorRequests + URL_VENDOR_HASH);
        request.timeout = 5;

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

        if(string.IsNullOrEmpty(hash))
        {
            Debug.Log("Requested VendorsHash is empty");
        }
        //If hash exists and is different than local hash
        else if (vendorInfo == null || hash != vendorInfo.Hash)
        {
            Debug.Log("VendorsHandler Hash difference, downloading new data.");
            yield return StartCoroutine(LoadVendorInfoRoutine());
        }
        else
        {
            Debug.Log("VendorsHash matches local cache.");
        }

        SetupRuntimeVendorData();
    }

    private IEnumerator LoadVendorInfoRoutine()
    {
        var request = UnityWebRequest.Get(urlVendorRequests + URL_VENDOR_JSON);
        request.timeout = 5;

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

        string data = request.downloadHandler.text;
        vendorInfo = JsonUtility.FromJson<GretaMarketVendorPackage>(data);

        if (vendorInfo == null)
        {
            Debug.LogError("Failed to create JSON from download.");
        }
        else
        {
            DownloadManager.Instance.SaveStringToFile(LOCAL_DATA_NAME, data);
        }
    }

    private void SetupRuntimeVendorData()
    {
        if (vendorInfo == null)
        {
            Debug.LogError("Cannot create runtime vendor data from null. This is caused when starting the app for the first time with no internet connection");
            Ready?.Invoke();
            return;
        }

        vendorDataCache = new Dictionary<int, RuntimeVendorData>();

        int i = 0;
        foreach (var vendor in vendorInfo.Vendors)
        {
            var data = new RuntimeVendorData(vendor, DownloadManager.Instance);
            vendorDataCache.Add(i++, data);
        }

        Debug.Log("VendorsHandler ready.");
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

            RuntimeVendorData data = new RuntimeVendorData(vendorInfo.Vendors[id], DownloadManager.Instance);
            vendorDataCache.Add(id, data);
            return data;
        }
    }

}

public class RuntimeVendorData
{
    private VendorData data;


    TextureRequest logoTex;
    TextureRequest mainTex;
    TextureRequest[] subTexs;

    public TextureRequest Logo => logoTex;
    public TextureRequest MainImage => mainTex;

    public TextureRequest[] SubImages => subTexs;


    public VendorData InternalData => data;

    public RuntimeVendorData(VendorData _data, DownloadManager downloadManager)
    {
        Debug.Log($"Started loading vendor {_data.Name}");

        data = _data;

        logoTex = GetTexture(downloadManager, data.LogoFile);
        mainTex = GetTexture(downloadManager, data.MainImageFile);

        subTexs = new TextureRequest[data.SubImagesFiles.Length];
        for (int i = 0; i < subTexs.Length; i++)
        {
            subTexs[i] = GetTexture(downloadManager, data.SubImagesFiles[i]);
        }
    }

    private TextureRequest GetTexture(DownloadManager downloadManager, string name)
    {
        if (!string.IsNullOrEmpty(name))
            return downloadManager.GetTexture(data.Directory, name);

        return null;
    }

    private Texture2D[] GetSubImageTextures()
    {
        Texture2D[] arr = new Texture2D[subTexs.Length];

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = subTexs[i] != null ? subTexs[i].Texture : null;
        }
        return arr;
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
    public string LinkYouTube;
    public string LinkPinterest;
    public string LinkZoom;

    public string LogoFile;
    public string MainImageFile;
    public string[] SubImagesFiles;

    public string Directory;
}

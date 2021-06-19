using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VendorsHander : MonoBehaviour
{

    [NaughtyAttributes.Button]
    public void TestGenerate()
    {
        GretaMarketVendorPackage pck = new GretaMarketVendorPackage();
        pck.Hash = "000";
        pck.Vendors = new VendorData[1];

        var data = new VendorData();
        data.Description = "DESC";
        data.Name = "NAME";

        pck.Vendors[0] = data;

        string example = JsonUtility.ToJson(pck, prettyPrint: true);
        Debug.Log(example);
    }

}

[System.Serializable]
public class GretaMarketVendorPackage
{
    public string Hash;

    public VendorData[] Vendors;
}

[System.Serializable]
public struct VendorData
{
    public string Name;
    public string Description;
    public string LinkWebsite;
    public string LinkFacebook;
    public string LinkInstagram;
    public string LinkZoom;

    public string LogoID;
    public string MainImageID;
    public string[] SubImagesID;
}

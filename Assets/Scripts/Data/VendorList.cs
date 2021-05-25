using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "VendorList", menuName = "Data/One-Time Usage/VendorList", order = 0)]
public class VendorList : ScriptableObject
{

    public bool doesVendorExist(string key)
    {
        bool b = false;

        foreach (Vendor v in Vendors)
        {
            if (v.companyName == key)
                b = true;
        }
        return b;
    }

    public Sprite getVendorSprite(string key)
    {
        foreach (Vendor v in Vendors)
        {
            if (v.companyName == key)
                return v.companyIcon;
        }
        return null;
    }
    public List<Vendor> Vendors;
}
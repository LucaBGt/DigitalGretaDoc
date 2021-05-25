using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Vendor", menuName = "Data/Vendor Info/Vendor", order = 0)]
public class Vendor : ScriptableObject
{
    public string companyName;

    public Sprite companyIcon;

    public string description;

    public List<Link> links;
}

[System.Serializable]
public class Link
{
    public string Address;

    public Sprite icon;
}
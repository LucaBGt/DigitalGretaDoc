using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Vendor", menuName = "Data/Product Info/Product", order = 0)]
public class Product : ScriptableObject
{
    public string productName;

    [MultiLineProperty(10)] public string productDescription;
    public List<Sprite> icons;
}

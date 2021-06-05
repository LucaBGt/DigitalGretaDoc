using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GretaSettings : ScriptableObject
{
    [NaughtyAttributes.InfoBox("This should always be: Resources/Settings", NaughtyAttributes.EInfoBoxType.Warning)]

    public Texture2D[] Skins;
    public Material[] SkinsMaterials;

}

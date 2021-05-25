using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class CharacterMessage : NetworkMessage
{
    public int characterType;
    public string characterName;
    public Color bodyColor = Color.green;
    //public Material bodyMat;
}

public enum characterType
{
    visitor, vendor, admin
}
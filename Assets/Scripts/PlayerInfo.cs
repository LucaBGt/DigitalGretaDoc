using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class PlayerInfo : NetworkBehaviour
{
    [SyncVar]
    public string DisplayName;
    public SkinnedMeshRenderer _renderer;

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

}

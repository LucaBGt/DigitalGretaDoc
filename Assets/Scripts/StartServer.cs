using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Mirror;
[CommandInfo("Networking", "Start Host", "Start Host")]
public class StartServer : Command
{
    public NetworkManager gn;
    public override void OnEnter()
    {
        gn.StartHost();
        Continue();
    }
}

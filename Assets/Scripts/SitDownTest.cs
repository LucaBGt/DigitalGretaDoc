using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SitDownTest : NetworkBehaviour
{
    public bool isOccupied;

    public Transform getOff;

    Transform player;

    public void DoThing(Transform _player)
    {
        if (isOccupied)
        {

        }
        else
        {
            player = _player;
            Debug.Log("SitDown");
            isOccupied = true;
            _player.transform.position = this.transform.position;
            //player.transform.rotation = this.transform.rotation;
            this.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public void StopOccupation()
    {
        this.transform.localScale = new Vector3(5, 5, 5);
        isOccupied = false;
        if(player!=null && getOff!=null)
        player.position = getOff.position;

    }

}

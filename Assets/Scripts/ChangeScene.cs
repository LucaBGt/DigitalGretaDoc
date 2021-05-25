using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
//using Greta;
public class ChangeScene : NetworkBehaviour
{
    //public GretaNetwork2 greta;

    NetworkBehaviour mn;

    [Scene]
    public string loadScene;
    [Scene]

    public string unloadScene;

    private void Awake()
    {
        //greta = FindObjectOfType<GretaNetwork2>();
    }

    [Server]
    void OnTriggerEnter(Collider other)
    {
        //if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Loading {0}", subScene);
        if (other.gameObject.CompareTag("Player"))
        {
            NetworkIdentity networkIdentity = other.gameObject.GetComponent<NetworkIdentity>();
            NetworkServer.SendToClientOfPlayer(networkIdentity, new SceneMessage { sceneName = loadScene, sceneOperation = SceneOperation.LoadAdditive });
            NetworkServer.SendToClientOfPlayer(networkIdentity, new SceneMessage { sceneName = unloadScene, sceneOperation = SceneOperation.UnloadAdditive });
            //SceneManager.MoveGameObjectToScene(other.gameObject, SceneManager.GetSceneByName(loadScene));
        }
    }
}

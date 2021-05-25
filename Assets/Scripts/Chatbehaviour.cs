using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class Chatbehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;
    private static event Action<string> OnMessage;

    public Transform ReferencePosition;

    public state myState;

    public NetworkAnimator animator;
    PlayerControls playerControls;

    void Start()
    {
        animator = GetComponent<NetworkAnimator>();
        playerControls = GetComponent<PlayerControls>();
    }

    public enum state
    {
        free, talking, huggingopen, huggingclosing
    }

    public override void OnStartAuthority()
    {
        //chatUI.SetActive(true);

        OnMessage += HandleNewMessage;

    }

    public void EndHug(GameObject g)
    {
        StartCoroutine(EndHugAnimation(g));
    }

    IEnumerator EndHugAnimation(GameObject g)
    {
        g.transform.position = ReferencePosition.position;
        g.transform.rotation = ReferencePosition.rotation;

        NetworkAnimator na = g.GetComponent<NetworkAnimator>();

        myState = state.huggingclosing;

        playerControls.Canvas.SetActive(false);

        animator.animator.SetFloat("Hug", 2);
        yield return new WaitForSeconds(0.5f);
        na.animator.SetFloat("Hug", 2);

        yield return new WaitForSeconds(3.5f);

        myState = state.huggingclosing;
        na.animator.SetFloat("Hug", 0);
        playerControls.StartHug(false);

        playerControls.Canvas.SetActive(true);

        playerControls.HugToggleReset();
    }

    void StartEnd()
    {
        Debug.Log("Starting the End of the Hug...");
        GameData._endHugWithMe.Invoke(this.gameObject);
    }

    public void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!hasAuthority) { return; }
        OnMessage -= HandleNewMessage;
    }

    #region requestConnection

    [Client]
    public void RequestChat(GameObject other, string username)
    {
        CmdRequestConnection(other, "Hi!", username);
    }

    [Client]
    public void RequestHug(GameObject other, string username)
    {
        Debug.Log("Sending Hug...");
        CmdRequestHug(other, "Hi!", username);
    }

    [Command]
    public void CmdRequestConnection(GameObject other, string message, string username)
    {

        NetworkIdentity otherConnection = other.GetComponent<NetworkIdentity>();

        TargetRequestConnection(otherConnection.connectionToClient, $"[{connectionToClient.connectionId}]:  {message}", username);
    }

    [Command]
    public void CmdRequestHug(GameObject other, string message, string username)
    {

        Debug.Log("Requesting Hug Command...");

        NetworkIdentity otherConnection = other.GetComponent<NetworkIdentity>();

        TargetRequestHug(otherConnection.connectionToClient, $"[{connectionToClient.connectionId}]:  {message}", username);
    }

    [TargetRpc]
    private void TargetRequestConnection(NetworkConnection target, string message, string username)
    {
        if (myState != state.talking)
            Debug.Log(username + " wants to chat!");
    }

    [TargetRpc]
    private void TargetRequestHug(NetworkConnection target, string message, string username)
    {

        //DOESNT WORK BECAUSE THE SENDER GETS SEND, NOT THE RECIEVER (ME)
        /*if (myState == state.huggingopen)
        {*/
        Debug.Log("Hug Request by " + username + ".");
        StartEnd();
        //}
    }

    #endregion

    [Client]
    public void Send(string message)
    {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }
        if (string.IsNullOrWhiteSpace(message)) { return; }

        CmdSendMessage(message);

        inputField.text = string.Empty;
    }

    [Command]
    public void CmdSendMessage(string message)
    {
        RpcHandleMessage($"[{connectionToClient.connectionId}]:  {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }
}

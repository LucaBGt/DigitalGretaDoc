using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum PlayerState
{
    Walking, Looking, UI
}

public class PlayerBehaviour : NetworkBehaviour
{
    public NavMeshAgent _agent;
    public SkinnedMeshRenderer _renderer;
    public Transform UIParent;
    public Transform GameplayParent;

    [Header("Player Name")]
    public TextMeshPro playerNameText;
    public GameObject floatingInfo;

    public string tempName;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [Header("Inputs")]
    public bool isTurning;
    public bool isLeft;
    public float sensetivity;

    [Header("Internal State")]
    private PlayerState state;
    public PlayerState State
    {
        get => state;

        set
        {
            state = value;
            OnChangePlayerState?.Invoke(state);
        }
    }

    public System.Action<PlayerState> OnChangePlayerState;

    [Header("Screen Controls")]
    public CanvasGroup controlUI;

    //Local References
    Camera cam;

    void OnNameChanged(string _Old, string _New)
    {
        playerNameText.text = playerName;
    }

    private void Start()
    {
        cam = Camera.main;

        if (!isLocalPlayer)
        {
            Destroy(_agent);
            Destroy(GameplayParent.gameObject);
        }
    }

    public override void OnStartLocalPlayer()
    {
        string playerName = tempName;
        CmdSetupPlayer(playerName);
    }

    [Command]
    public void CmdSetupPlayer(string _name)
    {
        // player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
    }


    void Update()
    {
        if (isLocalPlayer)
        {
            if (State == PlayerState.UI)
                return;
            LocalPlayerUpdate();
        }
        else
        {
            //remote player update
            floatingInfo.transform.LookAt(cam.transform);
        }
    }

    private void LocalPlayerUpdate()
    {
        //If the mouse button is clicked/The Screen is tapped...
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }

    }

    private void OnClick()
    {
        //not over UI
        if (!IsPointerOverUIObject())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                if (State == PlayerState.Looking)
                {
                    ClickOnLooking(hit);
                }
                else if (State == PlayerState.Walking)
                {
                    ClickOnWalking(hit);
                }
            }
        }
    }

    private void ClickOnWalking(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out IInteractable interactable))
        {
            _agent.destination = interactable.GetInteractPosition();
        }
        else
        {
            _agent.destination = hit.point;
        }
    }

    private void ClickOnLooking(RaycastHit hit)
    {
        // Do something with the object that was hit by the raycast.
        if (hit.transform.TryGetComponent(out Door door))
        {
            //interact with door 
            /*
            door.ShowAnimation();
            StartCoroutine(ShowDoor());
            StartCoroutine(loadJSON(currentDoor.url));
            */
        }
    }

    private void FixedUpdate()
    {
        if (State != PlayerState.UI && isLocalPlayer)
            if (isTurning)
            {
                if (isLeft)
                {
                    transform.Rotate(0, -sensetivity, 0);
                }
                else
                {
                    transform.Rotate(0, sensetivity, 0);
                }
            }
    }

    private IEnumerator turnToDoor()
    {
        State = PlayerState.UI;
        controlUI.interactable = false;
        //LeanTween.rotate(this.gameObject, currentDoor.goalPosition.rotation.eulerAngles, 1);
        yield return new WaitForSeconds(1f);
        controlUI.interactable = true;
        State = PlayerState.Walking;
    }

    public void TurnLeft()
    {
        isLeft = true;
        isTurning = true;
    }

    public void TurnRight()
    {
        isLeft = false;
        isTurning = true;
    }

    public void StopTurning()
    {
        isTurning = false;
    }

    #region Helper Function

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    #endregion
}
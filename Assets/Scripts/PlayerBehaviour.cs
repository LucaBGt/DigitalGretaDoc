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

    public bool walkingToDoor = false;

    [Header("Screen Controls")]
    public CanvasGroup controlUI;

    [Header("Door UI")]

    public CanvasGroup doorUI;

    public RawImage image;

    //Local References
    Camera cam;
    Door currentDoor;
    stand_json stand;

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


        if (State != PlayerState.UI)
        {

            if (!isLocalPlayer)
            {

                ///
                /// Important Question: If there was a way to register an 
                /// de-register these "Floating Info Objects" into one 
                /// "Upate" Method, we could completely disable this
                /// whole script if it doesn't belong to the local player, 
                /// which might change the perfomance for the better.
                ///

                // make non-local players run this
                //Makes the names above the players "look at" the player camera
                floatingInfo.transform.LookAt(cam.transform);
                return;
            }
            else
            {

                //If the mouse button is clicked/The Screen is tapped...
                if (Input.GetMouseButtonDown(0))
                {
                    //... and it wasn't on any UI...
                    if (!IsPointerOverUIObject())
                    {
                        //... then, depending on the current player state...
                        if (State == PlayerState.Looking)
                        {
                            RaycastHit hit;
                            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out hit))
                            {
                                Transform objectHit = hit.transform;

                                // Do something with the object that was hit by the raycast.
                                if (objectHit.CompareTag("Door"))
                                {
                                    currentDoor = objectHit.gameObject.GetComponent<Door>();
                                    currentDoor.ShowAnimation();
                                    StartCoroutine(ShowDoor());
                                    StartCoroutine(loadJSON(currentDoor.url));
                                }
                            }
                        }
                        else if (State == PlayerState.Walking)
                        {
                            //Place the target object, walk with the NavMeshAgent, etc...
                            RaycastHit hit;
                            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out hit))
                            {
                                Transform objectHit = hit.transform;

                                // Do something with the object that was hit by the raycast.
                                if (objectHit.CompareTag("Door"))
                                {
                                    currentDoor = objectHit.gameObject.GetComponent<Door>();
                                    _agent.destination = currentDoor.GoalPosition;

                                    //When the walking path is finished, turnToDoor() should be excecuted.
                                    walkingToDoor = true;
                                }
                                else
                                {
                                    _agent.destination = hit.point;
                                }
                            }
                        }
                    }
                }

                // Check if we've reached the destination
                // We do this to exchange the "Stop" button to the "Turn around" button
                if (!_agent.pathPending)
                {
                    if (_agent.remainingDistance <= _agent.stoppingDistance)
                    {
                        if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                        {
                            //Change the button back to the turning button

                            if (walkingToDoor == true)
                            {

                            }
                        }
                    }
                }

            }
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

    public void isLooking(bool setLooking)
    {
        if (setLooking)
        {
            State = PlayerState.Looking;
        }
        else
        {
            State = PlayerState.Walking;
        }
    }

    #region Door Interaction Logic

    //Shows the Door UI
    IEnumerator ShowDoor()
    {
        doorUI.blocksRaycasts = true;

        yield return new WaitForSeconds(1.5f);

        //TO REIMPLEMENT
        //LeanTween.alphaCanvas(doorUI, 1, 1.5f);

        doorUI.interactable = true;
    }

    //Hides the Door UI
    public void EndDoor()
    {
        currentDoor.EndDoor();
        doorUI.interactable = false;
        doorUI.blocksRaycasts = false;


        //TO REIMPLEMENT
        //LeanTween.alphaCanvas(doorUI, 0, 1f);

    }

    //Downloads the Vendor's Info JSON from the server
    IEnumerator loadJSON(string URL)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            Debug.Log("Download image on progress" + www.progress);
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed");
        }
        else
        {
            Debug.Log("Download succes");
            string JSONString;
            JSONString = www.text;

            stand = JsonUtility.FromJson<stand_json>(JSONString);

            StartCoroutine(setImage(stand.image_url));

        }
    }

    //Set the Preview Image from the link provided by the JSON
    IEnumerator setImage(string URL)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            Debug.Log("Download image on progress" + www.progress);
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed");
        }
        else
        {
            Debug.Log("Download succes");
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(www.bytes);
            texture.Apply();


            image.texture = texture;

        }
    }

    #endregion

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



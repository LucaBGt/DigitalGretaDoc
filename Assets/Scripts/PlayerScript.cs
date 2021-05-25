using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace QuickStart
{
    public class PlayerScript : NetworkBehaviour
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

        #region CharacterCreation (Deprecated)

        //public string tempName;
        //public Color tempColor;
        //public int tempHat;
        //public int tempWings;
        //private Material playerMaterialClone;


        //[SyncVar(hook = nameof(OnColorChanged))]
        //public Color playerColor = Color.white;

        //[SyncVar(hook = nameof(OnHatChanged))]
        //public int hatNumber = 0;
        //public List<GameObject> Hats;

        //[SyncVar(hook = nameof(OnWingsChanged))]
        //public int wingNumber = 0;
        //public List<GameObject> Wings;

        #endregion

        [Header("Inputs")]
        public bool isTurning;
        public bool isLeft;
        public float sensetivity;

        [Header("Internal State")]
        public PlayerState playerState;
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

        #region Character Creation Methods (Deprecated)

        /*
                void OnColorChanged(Color _Old, Color _New)
                {
                    playerNameText.color = _New;
                    //playerMaterialClone = new Material(_renderer.material);
                    //playerMaterialClone.color = _New;
                    //_renderer.material = playerMaterialClone;
                }

                void OnHatChanged(int _Old, int _New)
                {
                    Hats[_Old].SetActive(false);
                    Hats[_New].SetActive(true);
                }

                void OnWingsChanged(int _Old, int _New)
                {
                    Wings[_Old].SetActive(false);
                    Wings[_New].SetActive(true);
                }
        */
        #endregion

        public override void OnStartLocalPlayer()
        {

            if (!isLocalPlayer)
            {
                Destroy(_agent);
                Destroy(UIParent.gameObject);
                Destroy(GameplayParent.gameObject);
            }

            //Camera.main.transform.SetParent (transform);
            //Camera.main.transform.localPosition = new Vector3 (0, 0, 0);

            //floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
            //floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string playerName = tempName;

            //Character Creation Stuff (Deprecated)
            //Color color = tempColor;
            //int hat = tempHat;
            //int wings = tempWings;

            //CmdSetupPlayer(name, color, hat, wings);
            CmdSetupPlayer(playerName);

            cam = Camera.main;
        }

        [Command]
        //public void CmdSetupPlayer(string _name, Color _col, int _hat, int _wings
        public void CmdSetupPlayer(string _name)
        {
            // player info sent to server, then server updates sync vars which handles it on all clients
            playerName = _name;

            //Character Creation Stuff (Deprecated)
            //playerColor = _col;
            //hatNumber = _hat;
            //wingNumber = _wings;
        }

        private void Awake()
        {

        }

        void Update()
        {
            if (playerState != PlayerState.ui)
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
                            if (playerState == PlayerState.looking)
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
                            else if (playerState == PlayerState.walking)
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
                                        _agent.destination = currentDoor.goalPosition.position;

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
            if (playerState != PlayerState.ui && isLocalPlayer)
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
            playerState = PlayerState.ui;
            controlUI.interactable = false;
            LeanTween.rotate(this.gameObject, currentDoor.goalPosition.rotation.eulerAngles, 1);
            yield return new WaitForSeconds(1f);
            controlUI.interactable = true;
            playerState = PlayerState.walking;
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
                playerState = PlayerState.looking;
            }
            else
            {
                playerState = PlayerState.walking;
            }
        }

        #region Door Interaction Logic

        //Shows the Door UI
        IEnumerator ShowDoor()
        {
            doorUI.blocksRaycasts = true;

            yield return new WaitForSeconds(1.5f);

            LeanTween.alphaCanvas(doorUI, 1, 1.5f);

            //doorUI.alpha = 1;

            doorUI.interactable = true;
        }

        //Hides the Door UI
        public void EndDoor()
        {
            currentDoor.EndDoor();
            doorUI.interactable = false;
            doorUI.blocksRaycasts = false;

            //und doorUI.alpha = 0;

            LeanTween.alphaCanvas(doorUI, 0, 1f);

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

    public enum PlayerState
    {
        walking, looking, ui
    }

}
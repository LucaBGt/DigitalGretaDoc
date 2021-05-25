using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Pixelplacement;
using UnityEngine.UI;

public class PlayerControls : NetworkBehaviour
{
    public bool Authority;

    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    // get the incremental value of mouse moving
    private Vector2 mouseLook;
    // smooth the mouse moving
    private Vector2 smoothV;

    private Vector3 inputVector;

    private NetworkIdentity otherPlayer;

    /// <summary>The Current control Scheme Attached to the player</summary>
    public controlScheme currentControlScheme;

    [Header("Control UI Panels")]
    public GameObject mobileButtons;
    public GameObject fpsView;
    public GameObject menu;

    public GameObject turnAroundButton;
    MenuCanvas menuCanvas;
    public InfoCanvas infoCanvas;
    public ProductCanvas productCanvas;

    [Header("On-Object References")]
    [SerializeField] private Chatbehaviour chatbehaviour;
    [SerializeField] private PlayerInfo info;
    public NetworkAnimator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rigidbody;

    [Header("Other References")]
    [SerializeField] private Transform camera;
    [SerializeField] private Transform camera2;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject GoalObject;
    GameObject GoalInstance;
    private Camera mainCam;

    [Header("Settings")]
    public bool canMove = true;

    bool turnCamLeft = false;
    bool turnCamRight = false;
    public int speed = 2;

    public bool lookMode = false;

    public bool isOccupied;

    [Header("EmotionField")]
    public GameObject emotionPanel;
    public bool showEmotionPanel = false;
    public Button showEmotions;

    [Header("References")]
    public SitDownTest sdt;

    [Header("Hugging Stuff")]
    public Toggle hugToggle;
    public GameObject Canvas;
    private void Awake()
    {
        //GameData._endHugWithMe.AddListener(EndHug);
        DontDestroyOnLoad(this.gameObject);
        if (hasAuthority)
        {
            Debug.Log("I have Authority");
            GameData.changeControlScheme.AddListener(UpdateControlScheme);
            GameData.setCanMove.AddListener(UpdateCanMove);

            GameData._endHugWithMe.AddListener(EndHug);
            GameData.setLookMode.AddListener(SetLookMode);

            mainCam = camera.GetComponent<Camera>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Authority = hasAuthority;

        if (hasAuthority)
        {

            Debug.Log("I have Authority");
            GameData.changeControlScheme.AddListener(UpdateControlScheme);
            GameData.setCanMove.AddListener(UpdateCanMove);

            GameData._endHugWithMe.AddListener(EndHug);

            GoalInstance = Instantiate(GoalObject, transform.position, new Quaternion(0, 0, 0, 0));
            GoalInstance.SetActive(false);

            camera.gameObject.SetActive(true);
            mainCam = camera.gameObject.GetComponent<Camera>();
            foreach (Transform t in transform)
            {
                t.gameObject.layer = 8;
            }
            controlScheme loadedScheme = (controlScheme)PlayerPrefs.GetInt("currentControlScheme", 1);
            UpdateControlScheme(loadedScheme);

            //Hide other content
            menu.SetActive(false);
            menuCanvas = menu.GetComponent<MenuCanvas>();
        }
        else
        {
            //this.gameObject.tag = "NotPlayer";
            //chatbehaviour.enabled = false;
            Destroy(rigidbody);
            this.enabled = false;
            Destroy(camera.gameObject);
            Destroy(this);
            Destroy(canvas.gameObject);
            Destroy(agent);
        }
    }

    private void UpdateCanMove(bool can)
    {
        canMove = can;
    }

    private void UpdateControlScheme(controlScheme controlScheme)
    {

        Debug.Log("PLAYER: Control Scheme Changed!");

        currentControlScheme = controlScheme;

        switch (currentControlScheme)
        {

            case controlScheme.fps:
                Debug.Log("PLAYER: Changed to FPS");

                Cursor.lockState = CursorLockMode.Locked;
                //Cursor.visible = false;
                mobileButtons.SetActive(false);
                fpsView.SetActive(true);
                if (GoalInstance != null)
                    GoalInstance.SetActive(false);
                else
                {
                    GoalInstance = Instantiate(GoalObject, transform.position, new Quaternion(0, 0, 0, 0));
                    GoalInstance.SetActive(false);
                }
                agent.enabled = false;
                rigidbody.velocity = Vector3.zero;
                rigidbody.isKinematic = false;
                break;
            case controlScheme.mobile:
                Debug.Log("PLAYER: Changed to Mobile");

                Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
                mobileButtons.SetActive(true);
                fpsView.SetActive(false);
                Vector3 camRotation = camera.rotation.eulerAngles;
                camera.rotation = Quaternion.Euler(18, camRotation.y, camRotation.z);
                if (GoalInstance != null)
                    GoalInstance.SetActive(false);
                else
                {
                    GoalInstance = Instantiate(GoalObject, transform.position, new Quaternion(0, 0, 0, 0));
                    GoalInstance.SetActive(false);
                }
                agent.enabled = true;
                rigidbody.velocity = Vector3.zero;
                rigidbody.isKinematic = true;
                break;
            case controlScheme.maponly:
                Cursor.lockState = CursorLockMode.None;
                mobileButtons.SetActive(false);
                fpsView.SetActive(false);
                GoalInstance.SetActive(false);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (hasAuthority && lookMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                if (!IsPointerOverUIObject() && Physics.Raycast(ray, out hit, 10.0f))
                {
                    if (hit.collider.CompareTag("Interactable"))
                    {
                        sdt = hit.collider.gameObject.GetComponent<SitDownTest>();
                        sdt.DoThing(this.transform);

                        //Activate Thrid Person
                        camera.gameObject.SetActive(false);
                        camera2.gameObject.SetActive(true);

                        animator.animator.SetBool("Sit", true);
                        this.transform.Translate(0, -0.5f, 0);
                    }
                    if (hit.collider.CompareTag("Player"))
                    {

                        Debug.Log("Did Hit");
                        NetworkIdentity ni = hit.collider.gameObject.GetComponent<NetworkIdentity>();
                        Debug.Log("Requesting Hug");

                        chatbehaviour.RequestHug(hit.collider.gameObject, info.DisplayName);

                        StartCoroutine(DoHug(hit.collider.gameObject));

                    }
                    if (hit.collider.CompareTag("Info"))
                    {

                        VendorInfo vi = hit.collider.GetComponent<VendorInfo>();

                        Debug.Log(vi.vendor.companyName);

                        infoCanvas.gameObject.SetActive(true);
                        infoCanvas.Setup(vi.vendor);
                        UpdateCanMove(false);
                    }
                    if (hit.collider.CompareTag("Product"))
                    {
                        ProductHolder vi = hit.collider.GetComponent<ProductHolder>();

                        Debug.Log(vi.myProduct.productName);

                        productCanvas.gameObject.SetActive(true);
                        productCanvas.Setup(vi.myProduct);
                        UpdateCanMove(false);
                    }
                }
            }
        }

        if (hasAuthority && canMove)
        {

            switch (currentControlScheme)
            {
                case controlScheme.fps:

                    if (Input.GetButtonDown("Menu"))
                    {
                        //Menu Function
                        OpenMenu();
                    }
                    else if (Input.GetButtonDown("Map"))
                    {
                        //Menu Function
                    }
                    else if (Input.GetButtonDown("Emotes"))
                    {
                        //Menu Function
                        DoAnimation("Dance");
                    }

                    else if (Input.GetButtonDown("Talk"))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(camera.position, camera.TransformDirection(Vector3.forward), out hit, 20f))
                        {
                            Debug.DrawRay(camera.position, camera.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                            if (hit.collider.CompareTag("Player"))
                            {
                                Debug.Log("Did Hit");

                                NetworkIdentity ni = hit.collider.gameObject.GetComponent<NetworkIdentity>();
                                if (ni != otherPlayer)
                                {
                                    otherPlayer = ni;
                                    //request Chat
                                    chatbehaviour.RequestChat(hit.collider.gameObject, info.DisplayName);
                                }
                            }
                            else
                            {
                                Debug.Log("Not hit...");
                            }
                        }
                    }
                    DoCameraLook();
                    break;
                case controlScheme.mobile:
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        if (!IsPointerOverUIObject() && Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                        {
                            if (hit.collider.CompareTag("World"))
                            {
                                Tween.StopAll();
                                GoalInstance.transform.position = hit.point;
                                GoalInstance.SetActive(true);
                                agent.destination = hit.point;
                                GoalInstance.transform.up = hit.normal;
                                animator.animator.SetTrigger("Move");
                            }
                            else if (hit.collider.CompareTag("Stand"))
                            {
                                StandInstance si = hit.collider.gameObject.GetComponent<StandInstance>();

                                Vector3 pos = si.standFront.position;

                                GoalInstance.transform.position = pos;
                                GoalInstance.SetActive(true);
                                agent.destination = pos;
                                //GoalInstance.transform.up = pos.normal;
                                animator.animator.SetTrigger("Move");
                            }
                        }
                    }
                    break;
            }
        }
    }

    IEnumerator DoHug(GameObject partner)
    {

        Chatbehaviour otherCB = partner.GetComponent<Chatbehaviour>();

        this.transform.position = otherCB.ReferencePosition.position;
        this.transform.rotation = otherCB.ReferencePosition.rotation;
        camera.gameObject.SetActive(false);
        camera2.gameObject.SetActive(true);

        yield return new WaitForSeconds(4);

        StartHug(false);
    }

    void DoCameraLook()
    {
        // md is mosue delta
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        // the interpolated float result between the two float values
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        // incrementally add to the camera look
        mouseLook += smoothV;

        // vector3.right means the x-axis
        float newMouse = Mathf.Clamp(-mouseLook.y, -70, 70);
        camera.localRotation = Quaternion.AngleAxis(newMouse, Vector3.right);
        this.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, this.transform.up);
        //CmdMove(Quaternion.AngleAxis(mouseLook.x, this.transform.up));
    }

    public void CameraTurn(bool turnLeft)
    {
        if (turnLeft)
            camera.Rotate(0, -10, 0, Space.World);
        else
            camera.Rotate(0, 10, 0, Space.World);
    }

    public void CameraTurnHold(bool turnLeft)
    {
        if (turnLeft)
            turnCamLeft = true;
        else
            turnCamRight = true;

        Debug.Log("Turn!");
    }

    public void CameraTurnStop()
    {
        turnCamRight = false;
        turnCamLeft = false;
        Debug.Log("Turn End!");

    }

    public void StopMoving()
    {
        if (agent.enabled)
            agent.destination = this.transform.position;
        GoalInstance.SetActive(false);
        if (lookMode && sdt != null)
            sdt.StopOccupation();
    }

    public void OpenMenu()
    {
        canMove = false;
        menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        menuCanvas.Init();
    }

    public void TurnAround()
    {
        Tween.Rotate(this.transform, new Vector3(0, 180, 0), Space.World, 2, 0);
    }

    public void SetMove()
    {
        canMove = true;
    }

    public void ShowEmotions()
    {
        showEmotionPanel = !showEmotionPanel;
        emotionPanel.SetActive(showEmotionPanel);
    }

    public void DoAnimation(string key)
    {
        StopMoving();
        animator.animator.SetBool(key, true);
        canMove = false;
    }

    public void DoWave(bool _active)
    {
        StopMoving();
        animator.animator.SetBool("Wave", _active);
        canMove = !_active;
        showEmotions.interactable = !_active;

        if (_active)
        {
            camera.gameObject.SetActive(false);
            camera2.gameObject.SetActive(true);
        }
        else
        {
            camera.gameObject.SetActive(true);
            camera2.gameObject.SetActive(false);
        }
    }

    public void StartHug(bool _active)
    {

        Debug.Log("Hug Start!");
        StopMoving();
        if (_active)
        {
            animator.animator.SetFloat("Hug", 1);
            chatbehaviour.myState = Chatbehaviour.state.huggingopen;
        }
        else
        {
            animator.animator.SetFloat("Hug", 0);
            chatbehaviour.myState = Chatbehaviour.state.free;
        }
        canMove = !_active;
        showEmotions.interactable = !_active;

        if (_active)
        {
            camera.gameObject.SetActive(false);
            camera2.gameObject.SetActive(true);
        }
        else
        {
            camera.gameObject.SetActive(true);
            camera2.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (hasAuthority && canMove)
        {
            switch (currentControlScheme)
            {
                case controlScheme.fps:
                    float x = Input.GetAxis("Horizontal");
                    float y = Input.GetAxis("Vertical");

                    inputVector = new Vector3(x, 0, y);
                    transform.Translate((inputVector * Time.deltaTime * speed), Space.Self);
                    agent.nextPosition = transform.position;
                    //Vector3 relative = transform.InverseTransformDirection(transform.position + (inputVector * Time.deltaTime * speed));
                    if (x != 0 || y != 0)

                        animator.animator.SetBool("moving", true);
                    else
                        animator.animator.SetBool("moving", false);

                    animator.animator.SetFloat("x", x);

                    animator.animator.SetFloat("y", y);
                    break;
                case controlScheme.mobile:
                    //Debug.Log("Mobile Scheme!");

                    if (agent.velocity.sqrMagnitude == 0)
                    {
                        turnAroundButton.SetActive(true);
                        animator.animator.SetBool("moving", false);
                        animator.animator.SetFloat("y", 0);
                    }
                    else
                    {
                        turnAroundButton.SetActive(false);
                        animator.animator.SetBool("moving", true);
                        animator.animator.SetFloat("y", 2);
                    }


                    break;
            }
        }
        else
        {
            animator.animator.SetBool("moving", false);
        }

        if (turnCamLeft)
            transform.Rotate(0, -1, 0, Space.World);
        if (turnCamRight)
            transform.Rotate(0, 1, 0, Space.World);
    }

    public void SetLookMode(bool _set)
    {
        lookMode = !lookMode;
        UpdateCanMove(!lookMode);
        agent.enabled = !lookMode;
        if (lookMode)
        {

        }
        if (lookMode == false)
        {
            if (sdt != null)
            {
                sdt.StopOccupation();
                this.transform.Translate(0, .5f, 0);
                animator.animator.SetBool("Sit", false);
            }
            camera.gameObject.SetActive(true);
            camera2.gameObject.SetActive(false);
            animator.animator.SetBool("Sit", false);
        }
    }
    //When Touching UI
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void EndHug(GameObject g)
    {
        Debug.Log("Ending Hug!");
        //StartHug(false);
        chatbehaviour.EndHug(g);
    }

    public void HugToggleReset()
    {
        hugToggle.isOn = false;
    }

    public override void OnStopClient()
    {
        GoalObject.gameObject.SetActive(false);
    }

}
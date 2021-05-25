using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Pixelplacement;

public class SimpleControls_Offline : MonoBehaviour
{
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

    [Header("On-Object References")]
    //[SerializeField] private Chatbehaviour chatbehaviour;
    //[SerializeField] private PlayerInfo info;
    public Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rigidbody;

    [Header("Other References")]
    [SerializeField] private Transform Camera;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject GoalObject;
    GameObject GoalInstance;
    private Camera mainCam;

    [Header("Settings")]
    public bool canMove = true;
    public bool canTurn = true;

    bool turnCamLeft = false;
    bool turnCamRight = false;
    public int speed = 2;

    [SerializeField]
    LayerMask layerMask;

    [Header("EmotionField")]
    public GameObject emotionPanel;
    public bool showEmotionPanel = false;


    private void Awake()
    {
        Debug.Log("I have Authority");
        GameData.changeControlScheme.AddListener(UpdateControlScheme);
        GameData.setCanMove.AddListener(UpdateCanMove);
        GameData.setCanTurn.AddListener(UpdateCanTurn);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("I have Authority");
        GameData.changeControlScheme.AddListener(UpdateControlScheme);
        GameData.setCanMove.AddListener(UpdateCanMove);

        GoalInstance = Instantiate(GoalObject, transform.position, new Quaternion(0, 0, 0, 0));
        GoalInstance.SetActive(false);

        Camera.gameObject.SetActive(true);
        mainCam = Camera.gameObject.GetComponent<Camera>();
        foreach (Transform t in transform)
        {
            t.gameObject.layer = 8;
        }
        //controlScheme loadedScheme = (controlScheme)PlayerPrefs.GetInt("currentControlScheme", 1);
        //UpdateControlScheme(loadedScheme);

        //Hide other content
        menu.SetActive(false);
        menuCanvas = menu.GetComponent<MenuCanvas>();
    }

    private void UpdateCanMove(bool can)
    {
        canMove = can;
    }
    private void UpdateCanTurn(bool can)
    {
        canTurn = can;
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
                Cursor.visible = false;
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
                Cursor.visible = true;
                mobileButtons.SetActive(true);
                fpsView.SetActive(false);
                Vector3 camRotation = Camera.rotation.eulerAngles;
                Camera.rotation = Quaternion.Euler(18, camRotation.y, camRotation.z);
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
        if (canMove)
        {

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (!IsPointerOverUIObject() && Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider.CompareTag("World"))
                    {
                        Tween.StopAll();
                        GoalInstance.transform.position = hit.point;
                        GoalInstance.SetActive(true);
                        agent.destination = hit.point;
                        GoalInstance.transform.up = hit.normal;
                        animator.SetTrigger("Move");
                    }
                    else if (hit.collider.CompareTag("Stand"))
                    {
                        StandInstance si = hit.collider.gameObject.GetComponent<StandInstance>();

                        Vector3 pos = si.standFront.position;

                        GoalInstance.transform.position = pos;
                        GoalInstance.SetActive(true);
                        agent.destination = pos;
                        //GoalInstance.transform.up = pos.normal;
                        animator.SetTrigger("Move");
                    }
                }

            }
        }
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
        Camera.localRotation = Quaternion.AngleAxis(newMouse, Vector3.right);
        this.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, this.transform.up);
        //CmdMove(Quaternion.AngleAxis(mouseLook.x, this.transform.up));
    }

    public void CameraTurn(bool turnLeft)
    {
        if (turnLeft)
            Camera.Rotate(0, -10, 0, Space.World);
        else
            Camera.Rotate(0, 10, 0, Space.World);
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
    }

    public void OpenMenu()
    {
        canMove = false;
        menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        menuCanvas.Init();
    }

    public void TurnAround()
    {
        if (canTurn)
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
        animator.SetTrigger(key);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {

            if (agent.velocity.sqrMagnitude == 0)
            {
                turnAroundButton.SetActive(true);
                animator.SetBool("moving", false);
                animator.SetFloat("y", 0);
            }
            else
            {
                turnAroundButton.SetActive(false);
                animator.SetBool("moving", true);
                animator.SetFloat("y", 2);
            }
        }
        else
        {
            animator.SetBool("moving", false);
        }
        if (canTurn)
        {
            if (turnCamLeft)
                transform.Rotate(0, -1, 0, Space.World);
            if (turnCamRight)
                transform.Rotate(0, 1, 0, Space.World);
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

}
